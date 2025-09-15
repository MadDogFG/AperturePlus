using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Commands
{
    public class CreatePortfolioForUserCommand : IRequest<bool>
    {
        public Guid UserId { get; private set; }
        public CreatePortfolioForUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}