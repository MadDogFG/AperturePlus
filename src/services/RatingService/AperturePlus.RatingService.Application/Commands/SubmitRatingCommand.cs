using AperturePlus.RatingService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Commands
{
    public record class SubmitRatingCommand:IRequest<bool>
    {
        public Guid ActivityId { get; private set; }
        public Guid RateByUserId { get; private set; }
        public Guid RateToUserId { get; private set; }
        public RoleType RatedUserRole { get; private set; }
        public int Score { get; private set; }
        public string? Comments { get; private set; }

        public SubmitRatingCommand(Guid activityId, Guid rateByUserId, Guid rateToUserId, RoleType ratedUserRole, int score, string? comments)
        {
            ActivityId = activityId;
            RateByUserId = rateByUserId;
            RateToUserId = rateToUserId;
            RatedUserRole = ratedUserRole;
            Score = score;
            Comments = comments;
        }
    }
}
