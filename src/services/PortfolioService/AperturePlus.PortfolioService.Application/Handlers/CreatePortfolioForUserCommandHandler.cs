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
            var profolio = await portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (profolio != null)
            {
                return false;
            }
            profolio = Portfolio.CreatePortfolio(request.UserId);
            await portfolioRepository.AddAsync(profolio, cancellationToken);
        }
    }
}
