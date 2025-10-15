using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.DTOs
{
    public record PendingRatingDto(
     Guid PendingRatingId,
     Guid ActivityId,
     string ActivityTitle,
     Guid RateToUserId,
     string RateToUserName,
     string RatedUserRole
 );
}
