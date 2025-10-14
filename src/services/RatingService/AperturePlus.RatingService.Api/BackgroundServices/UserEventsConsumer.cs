
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Infrastructure.Persistence;
using Contracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AperturePlus.RatingService.Api.BackgroundServices
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

        public override async Task StartAsync(CancellationToken cancellationToken)//StartAsync只在启动时调用一次
        {
            channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("user_events", ExchangeType.Topic, true);//声明交换机
            var queueName = (await channel.QueueDeclareAsync()).QueueName;//声明一个临时队列
            await channel.QueueBindAsync(queueName, "user_events", "user.register", cancellationToken: cancellationToken);//绑定队列到交换机，监听注册事件

            var consumer = new AsyncEventingBasicConsumer(channel);//异步消费者
            consumer.ReceivedAsync += async (model, ea) =>//收到消息时触发
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    await ProcessEvent(ea.RoutingKey, message);//处理事件
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            await base.StartAsync(cancellationToken);
        }

        private async Task ProcessEvent(string routingKey, string message)
        {
            //BackgroundService是单例的，而DbContext是Scoped的，所以我们必须创建一个新的作用域来安全地解析DbContext。
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RatingServiceDbContext>();

            switch (routingKey)
            {
                case "user.register":
                    var regEvent = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message);
                    if (regEvent != null && !await dbContext.UserSummaries.AnyAsync(u => u.UserId == regEvent.UserId))
                    {
                        var userSummary = UserSummary.Create(regEvent.UserId, regEvent.UserName);
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
