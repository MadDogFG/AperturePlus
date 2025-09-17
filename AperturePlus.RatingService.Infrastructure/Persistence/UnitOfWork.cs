using AperturePlus.RatingService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RatingServiceDbContext dbContext;

        public UnitOfWork(RatingServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
