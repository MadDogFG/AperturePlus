using AperturePlus.ActivityService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ActivityServiceDbContext dbContext;

        public UnitOfWork(ActivityServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
