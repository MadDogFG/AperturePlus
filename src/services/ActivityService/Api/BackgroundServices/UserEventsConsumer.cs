
using AperturePlus.ActivityService.Domain.Entities;
using AperturePlus.ActivityService.Infrastructure.Persistence;
using Contracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace AperturePlus.ActivityService.Api.BackgroundServices
{
    public class UserEventsConsumer : BackgroundService//后台服务监听用户事件
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConnection connection;
        private IChannel channel;

        public UserEventsConsumer(IServiceScopeFactory scopeFactory, IConnection connection)
        {
            this.scopeFactory = scopeFactory;
            this.connection = connection;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("user_events", ExchangeType.Topic, true);
            var queueName = (await channel.QueueDeclareAsync()).QueueName;
            await channel.QueueBindAsync(queueName, "user_events","user.register",cancellationToken:cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
                try
                {
                    await ProcessEvent(ea.RoutingKey, message);
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    
                }
                catch (Exception ex) 
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            await base.StartAsync(cancellationToken);
        }

        private async Task ProcessEvent(string routingKey, string message)
        {
            // BackgroundService是单例的，而DbContext是Scoped的，所以我们必须创建一个新的作用域来安全地解析DbContext。
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ActivityServiceDbContext>();

            switch (routingKey)
            {
                case "user.registered":
                    var regEvent = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message);
                    if (regEvent != null && !await dbContext.UserSummaries.AnyAsync(u => u.UserId == regEvent.UserId))
                    {
                        var userSummary = new UserSummary(regEvent.UserId, regEvent.UserName);
                        await dbContext.UserSummaries.AddAsync(userSummary);
                    }
                    break;

                case "user.updated":
                    //未来更新事件
                    break;
            }
            await dbContext.SaveChangesAsync();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                return Task.Delay(Timeout.Infinite, stoppingToken);
            }
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel?.CloseAsync();
            channel?.Dispose();
            base.Dispose();
        }
    }
}
