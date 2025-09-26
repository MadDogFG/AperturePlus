using AperturePlus.ActivityService.Domain.Entities;
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
        public Decimal Fee { get; private set; } = 0; //活动费用，默认0
        public List<RoleRequirement> RoleRequirements { get; private set; } = new List<RoleRequirement>(); //角色需求列表
        public List<Participant> Participants { get; private set; } = new List<Participant>(); //参与者列表                                                                            
        public int TotalRequiredCount { get; private set; }   // 总需求人数
        public int ApprovedParticipantsCount { get; private set; }  // 已批准人数
        public int PendingParticipantsCount { get; private set; }    // 待审核人数
        public ActivityDto(Guid activityId, string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, PostedByUserDto postedByUser, string status, decimal fee, List<RoleRequirement> roleRequirements, List<Participant> participants, int totalRequiredCount,int approvedParticipantsCount,int pendingParticipantsCount)
        {
            ActivityId = activityId;
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
            PostedByUser = postedByUser;
            Status = status;
            Fee = fee;
            RoleRequirements = roleRequirements;
            Participants = participants;
            TotalRequiredCount = totalRequiredCount;
            ApprovedParticipantsCount = approvedParticipantsCount;
            PendingParticipantsCount = pendingParticipantsCount;
        }
    }
}
