using AperturePlus.PortfolioService.Application.DTOs;
using AperturePlus.PortfolioService.Application.Interfaces;
using AperturePlus.PortfolioService.Application.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Handlers
{
    public class GetPortfolioByUserIdQueryHandler : IRequestHandler<GetPortfolioByUserIdQuery, PortfolioDto>
    {
        private readonly IPortfolioRepository portfolioRepository;

        public GetPortfolioByUserIdQueryHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<PortfolioDto> Handle(GetPortfolioByUserIdQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (portfolio == null)
            {
                throw new Exception("该用户未创建作品集空间");
            }
            return new PortfolioDto(portfolio.PortfolioId, portfolio.UserId, portfolio.Galleries);
        }
    }
}
