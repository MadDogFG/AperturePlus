using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Infrastructure.Persistence;

namespace AperturePlus.RatingService.Infrastructure.Repositories
{
    public class PendingRatingRepository : IPendingRatingRepository
    {
        private readonly RatingServiceDbContext dbContext;

        public PendingRatingRepository(RatingServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddRangeAsync(IEnumerable<PendingRating> pendingRatings, CancellationToken cancellationToken)
        {
            if(pendingRatings == null || !pendingRatings.Any())//防止传入空集合
            {
                return;
            }
            await dbContext.AddRangeAsync(pendingRatings,cancellationToken);
        }
    }
}
