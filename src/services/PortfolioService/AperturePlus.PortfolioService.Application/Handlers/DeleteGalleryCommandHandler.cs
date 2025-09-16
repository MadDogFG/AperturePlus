using AperturePlus.PortfolioService.Application.Commands;
using AperturePlus.PortfolioService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Handlers
{
    public class DeleteGalleryCommandHandler : IRequestHandler<DeleteGalleryCommand, bool>
    {
        private readonly IPortfolioRepository portfolioRepository;

        public DeleteGalleryCommandHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<bool> Handle(DeleteGalleryCommand request, CancellationToken cancellationToken)
        {
            var portfolio = await portfolioRepository.GetByUserIdAsync(request.UserId,cancellationToken);
            if (portfolio == null) 
            {
                return false;
            }
            try
            {
                portfolio.DeleteGallery(request.GalleryId);
            }
            catch (Exception)
            {

                return false;
            }
            return await portfolioRepository.UpdateAsync(portfolio,cancellationToken);
        }
    }
}
