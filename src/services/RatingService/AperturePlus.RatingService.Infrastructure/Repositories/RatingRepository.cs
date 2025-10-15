using AperturePlus.RatingService.Application.DTOs;
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Domain.ValueObjects;
using AperturePlus.RatingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Infrastructure.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly RatingServiceDbContext dbContext;

        public RatingRepository(RatingServiceDbContext context)
        {
            dbContext = context;
        }

        public async Task AddRangeAsync(IEnumerable<Rating> ratings, CancellationToken cancellationToken)
        {
            await dbContext.Ratings.AddRangeAsync(ratings, cancellationToken);
        }

        public void Update(Rating rating)
        {
            dbContext.Ratings.Update(rating);
        }

        public Task<Rating?> GetByIdAsync(Guid ratingId, CancellationToken cancellationToken)
        {
            return dbContext.Ratings.FindAsync(new object[] { ratingId }, cancellationToken).AsTask();
        }

        public Task<Rating?> GetPendingRatingForSubmitAsync(Guid ratingId, Guid rateByUserId, CancellationToken cancellationToken)
        {
            return dbContext.Ratings.FirstOrDefaultAsync(r =>
                r.RatingId == ratingId &&
                r.RateByUserId == rateByUserId &&
                r.Status == RatingStatus.Pending,
                cancellationToken);
        }

        public async Task<IEnumerable<PendingRatingDto>> GetPendingRatingsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await (
                from r in dbContext.Ratings
                join us in dbContext.UserSummaries on r.RateToUserId equals us.UserId
                join act in dbContext.ActivitySummaries on r.ActivityId equals act.ActivityId
                where r.RateByUserId == userId && r.Status == RatingStatus.Pending
                select new PendingRatingDto(
                    r.RatingId,
                    r.ActivityId,
                    act.ActivityTitle,
                    r.RateToUserId,
                    us.UserName,
                    r.RatedUserRole.ToString()
                )
            ).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ReceivedRatingDto>> GetReceivedRatingsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await (
                from r in dbContext.Ratings
                join us in dbContext.UserSummaries on r.RateByUserId equals us.UserId
                where r.RateToUserId == userId && r.Status == RatingStatus.Completed
                select new ReceivedRatingDto(
                    r.RatingId,
                    r.Score.Value, // .Value because Score is nullable
                    r.Comments,
                    r.RateByUserId,
                    us.UserName,
                    r.SubmittedAt.Value // .Value because SubmittedAt is nullable
                )
            ).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SentRatingDto>> GetSentRatingsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await (
               from r in dbContext.Ratings
               join us in dbContext.UserSummaries on r.RateToUserId equals us.UserId
               where r.RateByUserId == userId && r.Status == RatingStatus.Completed
               select new SentRatingDto(
                   r.RatingId,
                   r.Score.Value,
                   r.Comments,
                   r.RateToUserId,
                   us.UserName,
                   r.SubmittedAt.Value
               )
           ).ToListAsync(cancellationToken);
        }

        public async Task<RatingStatsDto> GetRatingStatsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var ratings = await dbContext.Ratings
                .Where(r => r.RateToUserId == userId && r.Status == RatingStatus.Completed)
                .Select(r => r.Score)
                .ToListAsync(cancellationToken);

            if (!ratings.Any())
            {
                return new RatingStatsDto(0, 100.0);
            }

            int totalCount = ratings.Count;
            int positiveCount = ratings.Count(s => s.HasValue && s.Value >= 4);
            double positiveRate = (double)positiveCount / totalCount * 100;

            return new RatingStatsDto(totalCount, Math.Round(positiveRate, 1));
        }
    }
}
