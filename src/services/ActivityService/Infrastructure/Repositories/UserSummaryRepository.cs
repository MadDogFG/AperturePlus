using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Domain.Entities;
using AperturePlus.ActivityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Infrastructure.Repositories
{
    public class UserSummaryRepository : IUserSummaryRepository
    {
        private readonly ActivityServiceDbContext dbContext;
        public UserSummaryRepository(ActivityServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddUserSummaryAsync(UserSummary userSummary, CancellationToken cancellationToken)
        {
            await dbContext.AddAsync(userSummary, cancellationToken);
        }

        public void DeleteUserSummaryAsync(UserSummary userSummary)
        {
            dbContext.Remove(userSummary);
        }

        public async Task<List<UserSummary>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.UserSummaries.ToListAsync(cancellationToken);
        }

        public async Task<UserSummary?> GetByIdAsync(Guid userSummaryId, CancellationToken cancellationToken)
        {
            return await dbContext.UserSummaries.FirstOrDefaultAsync(a => a.UserId == userSummaryId, cancellationToken);
        }

        public void UpdateUserSummary(UserSummary userSummary)
        {
            dbContext.Update(userSummary);
        }
    }
}
