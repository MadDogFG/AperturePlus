using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Domain.Entities;
using AperturePlus.UserProfileService.Infrastructure.Persistence;
using Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace AperturePlus.UserProfileService.Api.BackgroundServices
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
            await channel.QueueBindAsync(queueName, "user_events","user.register",cancellationToken:cancellationToken);//绑定队列到交换机，监听注册事件
            await channel.QueueBindAsync(queueName, "user_events","user.roles.updated",cancellationToken:cancellationToken);//绑定队列到交换机，监听角色更新事件

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
            // BackgroundService 是单例，所以我们为每次消息处理创建一个新的作用域
            using var scope = scopeFactory.CreateScope();
            // 从这个临时的作用域中获取 IMediator
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            switch (routingKey)
            {
                case "user.register":
                    var regEvent = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message);
                    var result = await mediator.Send(new CreateUserProfileCommand(regEvent.UserId, regEvent.UserName, regEvent.Roles));
                    if (result == true)
                    {
                        Console.WriteLine($"用户 {regEvent.UserName} 的用户资料已创建，角色：{string.Join(", ", regEvent.Roles)}。");
                    }
                    else
                    {
                        Console.WriteLine($"用户 {regEvent.UserName} 的用户资料创建失败。");
                    }
                    break;
                    
                case "user.roles.updated":
                    var rolesUpdatedEvent = JsonSerializer.Deserialize<UserRolesUpdatedIntegrationEvent>(message);
                    var updateResult = await mediator.Send(new UpdateUserRolesCommand(rolesUpdatedEvent.UserId, rolesUpdatedEvent.Roles));
                    if (updateResult == true)
                    {
                        Console.WriteLine($"用户 {rolesUpdatedEvent.UserName} 的角色已更新为：{string.Join(", ", rolesUpdatedEvent.Roles)}。");
                    }
                    else
                    {
                        Console.WriteLine($"用户 {rolesUpdatedEvent.UserName} 的角色更新失败。");
                    }
                    break;
            }
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
