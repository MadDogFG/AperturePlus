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
    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, bool>
    {
        private readonly IPortfolioRepository portfolioRepository;

        public DeletePhotoCommandHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var portfolio = await portfolioRepository.GetByUserIdAsync(request.UserId,cancellationToken);
            if(portfolio == null)
            {
                return false;
            }
            var gallery = portfolio.Galleries.FirstOrDefault(g=>g.GalleryId==request.GalleryId);
            if(gallery == null)
            {
                return false;
            }
            foreach (var photoId in request.PhotoIds)
            {
                gallery.RemovePhoto(photoId);
            }
            return await portfolioRepository.UpdateAsync(portfolio,cancellationToken);
        }
    }
}
