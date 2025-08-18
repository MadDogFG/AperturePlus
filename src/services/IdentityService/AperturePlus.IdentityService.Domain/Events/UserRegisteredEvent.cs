using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Domain.Events
{
    public record class UserRegisteredEvent: INotification
    {
        public Guid UserId { get; }

        public UserRegisteredEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
