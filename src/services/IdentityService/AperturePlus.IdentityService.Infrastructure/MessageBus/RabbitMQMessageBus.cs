using AperturePlus.IdentityService.Application.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Infrastructure.MessageBus
{
    public class RabbitMQMessageBus : IMessageBus,IDisposable
    {
        private readonly IConnection connection;

        public RabbitMQMessageBus(IConnection connection)
        {
            this.connection = connection;
        }

        public void Dispose()
        {
            //让DI管理生命周期
        }

        public async Task Publish(string exchange, string routingKey, object data)
        {
            var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic,durable:true);//声明交换机，使用topic类型，并且持久化
            if (data == null)
            {
                return;
            }
            var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var properties = new BasicProperties();//消息的其他属性,比如消息头什么的
            properties.Persistent = true;//表示消息是持久化的,即使rabbitmq重启也不会丢失
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
