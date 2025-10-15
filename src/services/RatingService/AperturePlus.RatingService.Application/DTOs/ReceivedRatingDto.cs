using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.DTOs
{
    public record ReceivedRatingDto(Guid RatingId, int Score, string? Comments, Guid RateByUserId, string RateByUserName, DateTime SubmittedAt);

}
