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
            //筛选出所有“已完成”且“有评分”的分数
            var completedScores = await dbContext.Ratings
                .Where(r => r.RateToUserId == userId &&
                            r.Status == RatingStatus.Completed &&
                            r.Score.HasValue) //确保只包含有评分的记录
                .Select(r => r.Score.Value) //获取非空的 int 分数
                .ToListAsync(cancellationToken);

            //如果没有任何已评分的记录
            if (!completedScores.Any())
            {
                // 返回总数为 0，平均分为 0.0
                return new RatingStatsDto(0, 0.0);
            }

            //计算总数和平均分
            int totalCount = completedScores.Count;
            double averageScore = completedScores.Average();

            //将平均分转换为百分比
            double averagePercentage = averageScore * 10;

            return new RatingStatsDto(totalCount, Math.Round(averagePercentage, 1));
        }
    }
}
