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
        public List<string> Roles { get; set; } = new List<string>();
        public UserRegisteredIntegrationEvent(Guid userId, string userName, List<string> roles)
        {
            UserId = userId;
            UserName = userName;
            Roles = roles;
        }
    }
}
