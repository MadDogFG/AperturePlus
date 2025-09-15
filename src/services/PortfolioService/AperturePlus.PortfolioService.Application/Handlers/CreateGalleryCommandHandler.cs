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
    public class CreateGalleryCommandHandler : IRequestHandler<CreateGalleryCommand, Guid>
    {
        private readonly IPortfolioRepository portfolioRepository;
        public CreateGalleryCommandHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }
        public async Task<Guid> Handle(CreateGalleryCommand request, CancellationToken cancellationToken)
        {
            var profolio = await portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if(profolio == null)
            {
                throw new Exception("该用户未创建作品集空间");
            }
            var galleryId = profolio.CreateNewGallery(request.GalleryName);
            await portfolioRepository.UpdateAsync(profolio, cancellationToken);
            return galleryId;
        }
    }
}
