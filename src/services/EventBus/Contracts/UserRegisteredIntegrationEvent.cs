using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public record class UserRegisteredIntegrationEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public UserRegisteredIntegrationEvent(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
