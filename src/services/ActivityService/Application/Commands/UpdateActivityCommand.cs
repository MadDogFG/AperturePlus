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
        public Guid UserId { get; private set; } //更新活动的用户ID
        public string? ActivityTitle { get; set; }
        public string? ActivityDescription { get;  set; }
        public Location? ActivityLocation { get; set; }
        public DateTime? ActivityStartTime { get; set; }
        public Decimal? Fee { get; set; } //活动费用
        public List<RoleRequirement>? RoleRequirements { get; set; } = null; //角色需求列表

        public UpdateActivityCommand(Guid activityId,Guid userId)
        {
            ActivityId = activityId;
            UserId = userId;
        }
    }
}
