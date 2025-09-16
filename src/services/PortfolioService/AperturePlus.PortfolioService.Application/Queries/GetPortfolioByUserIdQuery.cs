using AperturePlus.PortfolioService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Queries
{
    public class GetPortfolioByUserIdQuery:IRequest<PortfolioDto>
    {
        public Guid UserId {  get; private set; }

        public GetPortfolioByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
