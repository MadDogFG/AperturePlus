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
        public Guid RateByUserId { get; private set; }
        public Guid RateToUserId { get; private set; }
        public RoleType RatedUserRole { get; private set; }
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
            if (score < 1 || score > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(score), "评分必须在1到5之间");
            }
            return new Rating(Guid.NewGuid(), activityId, rateByUserId, rateToUserId, ratedUserRole, score, comments);
        }
    }
}
