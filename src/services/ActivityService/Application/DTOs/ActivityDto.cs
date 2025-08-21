using AperturePlus.ActivityService.Domain.ValueObjects;

namespace AperturePlus.ActivityService.Application.DTOs
{
    public record class ActivityDto
    {
        public Guid ActivityId { get; init; } //活动ID
        public string ActivityTitle { get; init; } //活动标题
        public string ActivityDescription { get; init; } //活动描述
        public Location ActivityLocation { get; init; } //活动地点
        public DateTime ActivityStartTime { get; init; } //活动开始时间
        public PostedByUserDto PostedByUser { get; init; } //发布活动的用户信息
        public string Status { get; init; } //活动状态

        public ActivityDto(Guid activityId, string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, PostedByUserDto postedByUser, string status)
        {
            ActivityId = activityId;
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
            PostedByUser = postedByUser;
            Status = status;
        }
    }
}
