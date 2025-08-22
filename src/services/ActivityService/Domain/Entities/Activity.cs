using AperturePlus.ActivityService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Domain.Entities
{
    public record class Activity
    {
        public Guid ActivityId { get; private set; } //活动ID
        public string ActivityTitle { get; private set; } //活动标题
        public string ActivityDescription { get; private set; } //活动描述
        public Location ActivityLocation { get; private set; } //活动地点
        public DateTime ActivityStartTime { get; private set; } //活动开始时间
        public Guid PostedByUserId { get; private set; } //发布活动的用户ID
        public ActivityStatus Status { get; private set; } //活动状态

        private Activity() : base()
        {
        }

        private Activity(Guid activityId, string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, Guid postedByUserId, ActivityStatus status)
        {
            ActivityId = activityId;
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
            PostedByUserId = postedByUserId;
            Status = status;
        }

        public static Activity CreateActivity(string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, Guid postedByUserId)
        {
            return new Activity(
                activityId: Guid.NewGuid(),
                activityTitle: activityTitle,
                activityDescription: activityDescription,
                activityLocation: activityLocation,
                activityStartTime: activityStartTime,
                postedByUserId: postedByUserId,
                status: ActivityStatus.Open
            );
        }

        public void UpdateActivity(string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime)
        {
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
        }
    }
}
