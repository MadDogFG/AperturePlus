using AperturePlus.RatingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Interfaces
{
    public interface IPendingRatingRepository
    {
        Task AddRangeAsync(IEnumerable<PendingRating> pendingRatings, CancellationToken cancellationToken);
    }
}
