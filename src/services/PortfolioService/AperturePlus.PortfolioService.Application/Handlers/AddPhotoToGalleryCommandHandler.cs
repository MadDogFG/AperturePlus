using AperturePlus.PortfolioService.Application.Commands;
using AperturePlus.PortfolioService.Application.Interfaces;
using AperturePlus.PortfolioService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Handlers
{
    public class AddPhotoToGalleryCommandHandler:IRequestHandler<AddPhotoToGalleryCommand, List<Guid>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IFileStorageService fileStorageService;
        public AddPhotoToGalleryCommandHandler(IPortfolioRepository portfolioRepository, IFileStorageService fileStorageService)
        {
            this.portfolioRepository = portfolioRepository;
            this.fileStorageService = fileStorageService;
        }

        public async Task<List<Guid>> Handle(AddPhotoToGalleryCommand request, CancellationToken cancellationToken)
        {
            var portfolio = await portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if(portfolio == null)
            {
                throw new Exception("该用户未创建作品集空间");
            }
            var gallery = portfolio.Galleries.FirstOrDefault(g=>g.GalleryId==request.GalleryId);
            if(gallery == null)
            {
                throw new Exception("相册不存在");
            }
            List<Guid> newPhotoIds = gallery.Photos.Select(p => p.PhotoId).ToList();//获取现有的图片ID列表
            foreach (var file in request.Files)
            {
                var photoUrl = await fileStorageService.UploadFileAsync(file.FileStream, file.FileName, file.ContentType, cancellationToken);
                var photo = Photo.CreatePhoto(photoUrl);
                gallery.AddPhoto(photo);
                newPhotoIds.Add(photo.PhotoId);
            }
            await portfolioRepository.UpdateAsync(portfolio, cancellationToken);
            return newPhotoIds;
        }
    }
}
