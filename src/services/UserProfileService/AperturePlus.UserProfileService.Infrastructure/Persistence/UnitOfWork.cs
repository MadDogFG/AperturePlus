using AperturePlus.UserProfileService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserProfileDbContext dbContext;

        public UnitOfWork(UserProfileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
