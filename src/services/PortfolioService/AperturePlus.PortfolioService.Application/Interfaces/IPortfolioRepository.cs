using AperturePlus.PortfolioService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Interfaces
{
    public interface IPortfolioRepository
    {
        public Task<Portfolio?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        public Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken);

        public Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken cancellationToken);
    }
}
