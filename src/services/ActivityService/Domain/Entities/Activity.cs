using AperturePlus.ActivityService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
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
        public Decimal Fee { get; private set; } = 0; //活动费用，默认0
        public List<RoleRequirement> RoleRequirements { get; private set; } = new List<RoleRequirement>(); //角色需求列表
        public List<Participant> Participants { get; private set; } = new List<Participant>(); //参与者列表

        private Activity() : base()
        {
        }

        private Activity(Guid activityId, string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, Guid postedByUserId, ActivityStatus status, Decimal fee, List<RoleRequirement> roleRequirements)
        {
            ActivityId = activityId;
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
            PostedByUserId = postedByUserId;
            Status = status;
            Fee = fee;
            RoleRequirements = roleRequirements;
        }

        public static Activity CreateActivity(string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime, Guid postedByUserId, Decimal fee, List<RoleRequirement> roleRequirements)
        {
            return new Activity(
                activityId: Guid.NewGuid(),
                activityTitle: activityTitle,
                activityDescription: activityDescription,
                activityLocation: activityLocation,
                activityStartTime: activityStartTime,
                postedByUserId: postedByUserId,
                status: ActivityStatus.Open,
                fee: fee,
                roleRequirements: roleRequirements
            );
        }

        public void UpdateActivity(string activityTitle, string activityDescription, Location activityLocation, DateTime activityStartTime)
        {
            ActivityTitle = activityTitle;
            ActivityDescription = activityDescription;
            ActivityLocation = activityLocation;
            ActivityStartTime = activityStartTime;
        }

        public void CancelActivity()
        {
            if(Status==ActivityStatus.Completed||Status==ActivityStatus.Cancelled)
            {
                throw new InvalidOperationException("活动已取消或完成");
            }
            Status = ActivityStatus.Cancelled;
        }

        public void CompletedActivity()
        {
            if (Status == ActivityStatus.Completed || Status == ActivityStatus.Cancelled)
            {
                throw new InvalidOperationException("活动已取消或完成");
            }
            Status = ActivityStatus.Completed;
        }

        public void RequestJoinActivity(Guid applicationUserId, RoleType role)
        {
            if (Status != ActivityStatus.Open)
            {
                throw new InvalidOperationException("只能加入处于开放状态的活动");
            }
            if (!RoleRequirements.Any(r => r.Role == role))
            {
                throw new InvalidOperationException("所请求的角色不在活动需求中");
            }
            if (Participants.Count(p => p.Role == role && p.Status == ParticipantStatus.Approved) >= RoleRequirements.FirstOrDefault(r => r.Role == role).Quantity)
            {
                throw new InvalidOperationException("所请求的角色名额已满");
            }
            if(Participants.Any(p=>p.UserId == applicationUserId))
            {
                throw new InvalidOperationException("用户已申请过参加该活动");
            }
            Participants.Add(new Participant(applicationUserId, role));
        }

        public void ApproveParticipant(Guid userId, RoleType role)
        {
            var participant = Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                throw new InvalidOperationException("未找到该参与者");
            }
            if (Participants.Count(p => p.Role == role && p.Status == ParticipantStatus.Approved) >= RoleRequirements.FirstOrDefault(r => r.Role == role).Quantity)
            {
                throw new InvalidOperationException("角色名额已满不能批准");
            }
            participant.Approve();
        }

        public void RejectParticipant(Guid userId)
        {
            var participant = Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                throw new InvalidOperationException("未找到该参与者");
            }
            participant.Reject();
        }
    }
}
