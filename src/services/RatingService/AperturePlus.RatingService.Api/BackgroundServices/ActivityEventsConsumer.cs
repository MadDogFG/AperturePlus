
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Domain.ValueObjects;
using AperturePlus.RatingService.Infrastructure.Persistence;
using AperturePlus.RatingService.Infrastructure.Repositories;
using Contracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AperturePlus.RatingService.Api.BackgroundServices
{
    public class ActivityEventsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConnection connection;
        private IChannel channel;

        public ActivityEventsConsumer(IServiceScopeFactory scopeFactory, IConnection connection)
        {
            this.scopeFactory = scopeFactory;
            this.connection = connection;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)//StartAsync只在启动时调用一次
        {
            channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("activity_events", ExchangeType.Topic, true);//声明交换机
            var queueName = (await channel.QueueDeclareAsync()).QueueName;//声明一个临时队列
            await channel.QueueBindAsync(queueName, "activity_events", "activity.completed", cancellationToken: cancellationToken);//绑定队列到交换机，监听注册事件

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

        private async Task ProcessEvent(string routingKey, string message, CancellationToken cancellationToken)
        {
            //BackgroundService是单例的，而DbContext是Scoped的，所以我们必须创建一个新的作用域来安全地解析DbContext。
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RatingServiceDbContext>();
            var ratingRepository = scope.ServiceProvider.GetRequiredService<IRatingRepository>();
            var activitySummaryRepository = scope.ServiceProvider.GetRequiredService<IActivitySummaryRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            switch (routingKey)
            {
                case "activity.completed":
                    var cpEvent = JsonSerializer.Deserialize<ActivityCompletedIntegrationEvent>(message);
                    if (cpEvent != null && !await dbContext.Ratings.AnyAsync(p=>p.ActivityId== cpEvent.ActivityId))
                    {
                        var pendingRatingsToAdd = new List<Rating>();
                        foreach (var rateByUserId in cpEvent.Participants)
                        {
                            foreach(var rateToUserId in cpEvent.Participants)
                            {
                                if(rateByUserId.UserId != rateToUserId.UserId)
                                {
                                    var pendingRating = Rating.CreatePending(
                                        cpEvent.ActivityId,
                                        rateByUserId.UserId,
                                        rateToUserId.UserId,
                                        Enum.Parse<RoleType>(rateToUserId.Role)
                                        );
                                    pendingRatingsToAdd.Add(pendingRating);
                                }
                            }
                        }
                        await ratingRepository.AddRangeAsync(pendingRatingsToAdd, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    
                    if(cpEvent != null && !await dbContext.ActivitySummaries.AnyAsync(a => a.ActivityId == cpEvent.ActivityId))
                    {
                        var activitySummary = ActivitySummary.Create(cpEvent.ActivityId, cpEvent.ActivityTitle);
                        await activitySummaryRepository.AddAsync(activitySummary, cancellationToken);
                    }
                    break;

                case "activity.cancel":
                    //未来其他事件
                    break;
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
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
