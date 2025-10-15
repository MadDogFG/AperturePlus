using AperturePlus.RatingService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Domain.Entities
{
    public class Rating
    {
        public Guid RatingId { get; private set; }
        public Guid ActivityId { get; private set; }
        public Guid RateByUserId { get; private set; }//评价者
        public Guid RateToUserId { get; private set; }//被评价者
        public RoleType RatedUserRole { get; private set; }//被评价者的角色
        public RatingStatus Status { get; private set; }
        public int? Score { get; private set; }
        public string? Comments { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? SubmittedAt { get; private set; }//提交时间

        private Rating() { }
        public static Rating CreatePending(Guid activityId, Guid rateByUserId, Guid rateToUserId, RoleType ratedUserRole)
        {
            return new Rating
            {
                RatingId = Guid.NewGuid(),
                ActivityId = activityId,
                RateByUserId = rateByUserId,
                RateToUserId = rateToUserId,
                RatedUserRole = ratedUserRole,
                Status = RatingStatus.Pending, // 状态为 Pending
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Submit(int score, string? comments)
        {
            if (Status != RatingStatus.Pending)
            {
                throw new InvalidOperationException("已评价过，请勿重复评价");
            }
            if (score <= 1 || score >= 10)
            {
                throw new ArgumentOutOfRangeException(nameof(score), "评分要在1到10之间");
            }

            Score = score;
            Comments = comments;
            Status = RatingStatus.Completed; // 状态变为 Completed
            SubmittedAt = DateTime.UtcNow;
        }
    }
}
