using AperturePlus.PortfolioService.Application.Commands;
using AperturePlus.PortfolioService.Application.Interfaces;
using AperturePlus.PortfolioService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Handlers
{
    public class CreatePortfolioForUserCommandHandler : IRequestHandler<CreatePortfolioForUserCommand, bool>
    {
        private readonly IPortfolioRepository portfolioRepository;

        public CreatePortfolioForUserCommandHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<bool> Handle(CreatePortfolioForUserCommand request, CancellationToken cancellationToken)
        {
            var portfolio = await portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (portfolio != null)
            {
                return false;
            }
            portfolio = Portfolio.CreatePortfolio(request.UserId);
            await portfolioRepository.AddAsync(portfolio, cancellationToken);
            Console.WriteLine($"为用户 {request.UserId} 创建档案 {portfolio.PortfolioId}");
            return true;
        }
    }
}
