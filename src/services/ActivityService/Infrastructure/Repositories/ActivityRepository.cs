using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Domain.Entities;
using AperturePlus.ActivityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AperturePlus.ActivityService.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ActivityServiceDbContext dbContext;
        public async Task AddActivityAsync(Activity activity, CancellationToken cancellationToken)
        {
            await dbContext.AddAsync(activity, cancellationToken);
        }

        public void DeleteActivity(Activity activity)
        {
            dbContext.Remove(activity);
        }

        public async Task<List<Activity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Activities.ToListAsync(cancellationToken);
        }

        public async Task<Activity> GetByIdAsync(Guid activityId, CancellationToken cancellationToken)
        {
            return dbContext.Activities.Where(a => a.ActivityId == activityId).Single();
        }

        public void UpdateActivity(Activity activity)
        {
            dbContext.Update(activity);
        }
    }
}
