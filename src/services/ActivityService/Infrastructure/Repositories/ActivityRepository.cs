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

        public ActivityRepository(ActivityServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddActivityAsync(Activity activity, CancellationToken cancellationToken)
        {
            await dbContext.AddAsync(activity, cancellationToken);
        }

        public void DeleteActivity(Activity activity)
        {
            dbContext.Remove(activity);
        }

        //public async Task<List<Activity>> GetAllAsync(CancellationToken cancellationToken)
        //{
        //    return await dbContext.Activities.ToListAsync(cancellationToken);
        //}

        public async Task<Activity?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken)
        {
            return await dbContext.Activities.FirstOrDefaultAsync(a=>a.ActivityId==activityId,cancellationToken);
        }

        public void UpdateActivity(Activity activity)
        {
            dbContext.Update(activity);
        }
        public async Task<(IEnumerable<Activity> Activities, bool HasMore)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var skip = (page - 1) * pageSize;
            var items = await dbContext.Activities
                .OrderByDescending(a => a.ActivityStartTime)
                .Skip(skip)
                .Take(pageSize + 1) //多取一条用于判断是否有更多数据
                .ToListAsync(cancellationToken);
            bool hasMore = items.Count > pageSize;
            if (hasMore)
            {
                items.RemoveAt(items.Count - 1); //移除多取的那一条
            }
            return (items,hasMore);
        }

        public async Task<List<Activity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.Activities.Where(a => a.Participants.Any(p => p.UserId == userId)).ToListAsync(cancellationToken);
        }
    }
}
