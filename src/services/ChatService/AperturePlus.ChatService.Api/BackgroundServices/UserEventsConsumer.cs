using AperturePlus.ChatService.Application.Interfaces;
using AperturePlus.ChatService.Domain.Entities;
using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;

namespace AperturePlus.ChatService.Api.BackgroundServices
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
            await channel.QueueBindAsync(queueName, "user_events", "user.profile.updated", cancellationToken: cancellationToken);//监听用户更新事件

            var consumer = new AsyncEventingBasicConsumer(channel);//异步消费者
            consumer.ReceivedAsync += async (model, ea) =>//收到消息时触发
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    await ProcessEvent(ea.RoutingKey, message,cancellationToken);//处理事件
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

        private async Task ProcessEvent(string routingKey, string message,CancellationToken cancellationToken)
        {
            // 为每个消息创建一个新的依赖注入作用域
            using var scope = scopeFactory.CreateScope();
            var userSummaryRepository = scope.ServiceProvider.GetRequiredService<IUserSummaryRepository>();

            switch (routingKey)
            {
                case "user.register":
                    var regEvent = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message);
                    if (regEvent != null && await userSummaryRepository.GetByIdAsync(regEvent.UserId,cancellationToken) == null)
                    {
                        // 注册时，我们只知道用户名。UserProfileService 会在创建 Profile 后（或用户更新头像后）
                        // 发送 user.profile.updated 事件，届时我们再更新头像。
                        var userSummary = UserSummary.Create(regEvent.UserId, regEvent.UserName, "default_avatar_url_placeholder");
                        await userSummaryRepository.AddAsync(userSummary, cancellationToken);
                    }
                    break;

                case "user.profile.updated":
                    var updateEvent = JsonSerializer.Deserialize<UserProfileUpdatedIntegrationEvent>(message);
                    if (updateEvent != null)
                    {
                        var existingUser = await userSummaryRepository.GetByIdAsync(updateEvent.UserId, cancellationToken);
                        if (existingUser != null)
                        {
                            // 更新现有的 UserSummary
                            existingUser.Update(updateEvent.AvatarUrl);
                            await userSummaryRepository.UpdateAsync(existingUser, cancellationToken);
                        }
                        else
                        {
                            // 处理竞争条件：可能 profile.updated 事件比 register 事件先到
                            var newUserSummary = UserSummary.Create(updateEvent.UserId, updateEvent.UserName, updateEvent.AvatarUrl);
                            await userSummaryRepository.AddAsync(newUserSummary, cancellationToken);
                        }
                    }
                    break;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 保持后台服务持续运行
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
