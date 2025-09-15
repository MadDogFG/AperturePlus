
using AperturePlus.PortfolioService.Application.Commands;
using Contracts;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AperturePlus.PortfolioService.Api.BackgroundServices
{
    public class UserEventsConsumer : BackgroundService
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
            // BackgroundService 是单例，所以我们为每次消息处理创建一个新的作用域
            using var scope = scopeFactory.CreateScope();
            // 从这个临时的作用域中获取 IMediator
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            if (routingKey == "user.register")
            {
                var regEvent = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message);
                await mediator.Send(new CreatePortfolioForUserCommand(regEvent.UserId));
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

