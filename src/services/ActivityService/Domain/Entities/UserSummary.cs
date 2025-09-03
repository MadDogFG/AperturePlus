using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Domain.Entities
{
    public record class UserSummary
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }

        public UserSummary(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
