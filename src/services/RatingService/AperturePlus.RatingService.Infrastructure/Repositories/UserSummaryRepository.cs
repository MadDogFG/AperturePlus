using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Infrastructure.Repositories
{
    public class UserSummaryRepository:IUserSummaryRepository
    {
        private readonly RatingServiceDbContext dbContext;
        public UserSummaryRepository(RatingServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(ActivitySummary activitySummary, CancellationToken cancellationToken)
        {
            await dbContext.ActivitySummaries.AddAsync(activitySummary, cancellationToken);
        }
        public async Task<ActivitySummary?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.ActivitySummaries.FindAsync(userId, cancellationToken);
        }
    }
}
