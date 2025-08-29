using AperturePlus.ActivityService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Commands
{
    public class RequestJoinActivityCommand:IRequest<bool>
    {
        public Guid ActivityId { get; private set; }
        public Guid UserId { get; private set; }
        public RoleType Role {  get; private set; }

        public RequestJoinActivityCommand(Guid activityId, Guid userId,RoleType role)
        {
            ActivityId = activityId;
            UserId = userId;
            Role = role;
        }
    }
}
