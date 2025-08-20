using AperturePlus.ActivityService.Application.DTOs;
using AperturePlus.ActivityService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Commands
{
    public class CreateActivityCommand:IRequest<CreateActivityResult>
    {
        public string ActivityTitle { get; private set; } //活动标题
        public string ActivityDescription { get; private set; } //活动描述
        public Location ActivityLocation { get; private set; } //活动地点
        public DateTime ActivityStartTime { get; private set; } //活动开始时间
        public Guid PostedByUserId { get; private set; } //发布活动的用户ID
        public CreateActivityCommand(string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, Guid postedByUserId)
        {
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
            PostedByUserId = postedByUserId;
        }

    }
}
