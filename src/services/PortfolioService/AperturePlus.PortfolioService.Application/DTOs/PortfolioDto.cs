using AperturePlus.PortfolioService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.DTOs
{
    public record class PortfolioDto
    {
        public Guid PortfolioDtoId { get; private set; }
        public Guid UserId { get; private set; }
        public IReadOnlyCollection<Gallery>? Galleries { get; private set; }

        public PortfolioDto(Guid portfolioDtoId, Guid userId, IReadOnlyCollection<Gallery>? galleries)
        {
            PortfolioDtoId = portfolioDtoId;
            UserId = userId;
            Galleries = galleries;
        }
    }
}
