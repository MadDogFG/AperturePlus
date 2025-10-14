using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Infrastructure.Repositories
{
    public class ActivitySummaryRepository
    {
        private readonly RatingServiceDbContext dbContext;
        public ActivitySummaryRepository(RatingServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(ActivitySummary activitySummary, CancellationToken cancellationToken)
        {
            await dbContext.ActivitySummaries.AddAsync(activitySummary, cancellationToken);
        }
        public async Task<ActivitySummary?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken)
        {
            return await dbContext.ActivitySummaries.FindAsync(activityId, cancellationToken);
        }
    }
}
