using AperturePlus.RatingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Interfaces
{
    public interface IRatingRepository
    {
        Task AddAsync(Rating rating, CancellationToken cancellationToken);
        Task<IEnumerable<Rating>> GetRatingstoUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<Rating>> GetRatingsbyUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
