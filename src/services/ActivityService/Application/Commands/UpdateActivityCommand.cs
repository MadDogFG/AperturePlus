using AperturePlus.ActivityService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Commands
{
    public class UpdateActivityCommand:IRequest<bool>
    {
        public Guid ActivityId { get; private set; } //活动ID
        public string ActivityTitle { get; private set; }
        public string ActivityDescription { get; private set; }
        public Location ActivityLocation { get; private set; }
        public DateTime ActivityStartTime { get; private set; }
        public Guid UserId { get; private set; } //更新活动的用户ID
        public UpdateActivityCommand(Guid activityId, string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, Guid userId)
        {
            ActivityId = activityId;
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
            UserId = userId;
        }
    }
}
