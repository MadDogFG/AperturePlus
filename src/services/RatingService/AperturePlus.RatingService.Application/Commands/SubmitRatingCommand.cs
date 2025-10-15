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
        //评价记录的唯一ID，由前端从“待评价列表”中获取
        public Guid RatingId { get; init; }

        //评价者的ID，从Token中获取，用于安全校验
        public Guid RateByUserId { get; init; }

        public int Score { get; init; }
        public string? Comments { get; init; }

        public SubmitRatingCommand(Guid ratingId, Guid rateByUserId, int score, string? comments)
        {
            RatingId = ratingId;
            RateByUserId = rateByUserId;
            Score = score;
            Comments = comments;
        }
    }
}
