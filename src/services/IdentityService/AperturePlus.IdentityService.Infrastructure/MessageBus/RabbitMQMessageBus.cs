using AperturePlus.IdentityService.Application.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Infrastructure.MessageBus
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly IConnection connection;
        private readonly IChannel channel;

        public RabbitMQMessageBus(IConnection connection, IChannel channel)
        {
            this.connection = connection;
            this.channel = channel;
        }

        public async Task Publish(string exchange, string routingKey, object data)
        {
            if (data == null)
            {
                return;
            }
            var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var properties = new BasicProperties();//消息的其他属性,比如消息头什么的
            try
            {
                await channel.BasicPublishAsync(exchange, routingKey, false, properties, body);//mandatory设为false表示如果没有队列与该routingKey匹配，消息将被丢弃，设置为true则会返回给生产者
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
