using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
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

        public RatingRepository(RatingServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Rating rating, CancellationToken cancellationToken)
        {
            if (rating == null)
            {
                return;
            }
            await dbContext.AddAsync(rating, cancellationToken);
        }

        public async Task<IEnumerable<Rating>> GetRatingstoUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.Ratings.Where(r => r.RateToUserId == userId).ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Rating>> GetRatingsbyUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.Ratings.Where(r => r.RateByUserId == userId).ToListAsync(cancellationToken);
        }
    }
}
