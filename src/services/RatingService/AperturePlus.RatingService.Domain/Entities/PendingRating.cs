using AperturePlus.RatingService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Domain.Entities
{
    public class PendingRating
    {
        public Guid PendingRatingId { get; private set; }
        public Guid ActivityId { get; private set; }
        public Guid RateByUserId { get; private set; }//应评分者
        public Guid RateToUserId { get; private set; }//应被评分者
        public RoleType RatedUserRole { get; private set; }//被评分者当时的角色

        private PendingRating() { }

        public static PendingRating Create(Guid activityId, Guid rateByUserId, Guid rateToUserId, RoleType ratedUserRole)
        {
            if (rateByUserId == rateToUserId)
            {
                throw new InvalidOperationException("不能给自己评分");
            }

            return new PendingRating
            {
                PendingRatingId = Guid.NewGuid(),
                ActivityId = activityId,
                RateByUserId = rateByUserId,
                RateToUserId = rateToUserId,
                RatedUserRole = ratedUserRole
            };
        }
    }
}
