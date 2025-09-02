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

        public void Publish(string exchange, string routingKey, object data)
        {
            throw new NotImplementedException();
        }
    }
}
