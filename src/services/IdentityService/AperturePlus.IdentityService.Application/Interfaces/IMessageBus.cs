using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Interfaces
{
    public interface IMessageBus
    {
        public Task Publish(string exchange, string routingKey, object data);
    }
}
