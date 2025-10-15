using AperturePlus.RatingService.Application.DTOs;
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
        Task AddRangeAsync(IEnumerable<Rating> ratings, CancellationToken cancellationToken);
        void Update(Rating rating);
        Task<Rating?> GetByIdAsync(Guid ratingId, CancellationToken cancellationToken);
        Task<IEnumerable<PendingRatingDto>> GetPendingRatingsAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<ReceivedRatingDto>> GetReceivedRatingsAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<SentRatingDto>> GetSentRatingsAsync(Guid userId, CancellationToken cancellationToken);
        Task<RatingStatsDto> GetRatingStatsAsync(Guid userId, CancellationToken cancellationToken);
        Task<Rating?> GetPendingRatingForSubmitAsync(Guid ratingId, Guid rateByUserId, CancellationToken cancellationToken);
    }
}
