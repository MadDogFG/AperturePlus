using AperturePlus.PortfolioService.Application.Interfaces;
using AperturePlus.PortfolioService.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<Portfolio> portfoliosCollection;
        public PortfolioRepository(IMongoCollection<Portfolio> portfoliosCollection)
        {
            this.portfoliosCollection = portfoliosCollection;
        }
        public async Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken)
        {
            await portfoliosCollection.InsertOneAsync(portfolio, null, cancellationToken);
        }

        public async Task<Portfolio?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await portfoliosCollection.Find(p => p.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken cancellationToken)
        {
            var result = await portfoliosCollection.ReplaceOneAsync(
                p => p.PortfolioId == portfolio.PortfolioId,
                portfolio,
                cancellationToken:cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
