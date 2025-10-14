using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Domain.Entities
{
    public class UserSummary
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }

        private UserSummary() { }

        public static UserSummary Create(Guid userId, string userName)
        {
            return new UserSummary { UserId = userId, UserName = userName };
        }

        public void Update(string userName)
        {
            UserName = userName;
        }
    }
}
