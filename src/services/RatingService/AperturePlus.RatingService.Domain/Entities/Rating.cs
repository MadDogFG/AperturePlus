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
        public int Score { get; private set; }
        public string? Comments { get; private set; }
        public DateTime CreatedAt { get; private set; } 

        private Rating(Guid ratingId, Guid activityId, Guid rateByUserId, Guid rateToUserId, RoleType ratedUserRole, int score, string? comments)
        {
            RatingId = ratingId;
            ActivityId = activityId;
            RateByUserId = rateByUserId;
            RateToUserId = rateToUserId;
            RatedUserRole = ratedUserRole;
            Score = score;
            Comments = comments;
            CreatedAt = DateTime.UtcNow;
        }

        public static Rating CreateRating(Guid activityId, Guid rateByUserId, Guid rateToUserId, RoleType ratedUserRole, int score, string? comments)
        {
            if (score <= 1 || score >= 10)
            {
                throw new ArgumentOutOfRangeException(nameof(score), "评分必须在1到10之间");
            }
            return new Rating(Guid.NewGuid(), activityId, rateByUserId, rateToUserId, ratedUserRole, score, comments);
        }
    }
}
