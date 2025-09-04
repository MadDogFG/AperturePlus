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

        private UserSummary() : base()
        {
        }

        private UserSummary(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }

        public static UserSummary CreateUserSummary(Guid userId, string userName)
        {
            return new UserSummary(userId, userName);
        }

        public void UpdateUserName(string userName)
        {
            UserName = userName;
        }
    }
}
