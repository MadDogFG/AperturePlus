using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Commands
{
    public class CompletedActivityCommand:IRequest<bool>
    {
        public Guid ActivityId { get; private set; }//取消的活动ID
        public Guid UserId { get; private set; }//取消活动的用户ID，用于确认权限

        public CompletedActivityCommand(Guid activityId, Guid userId)
        {
            ActivityId = activityId;
            UserId = userId;
        }
    }
}
