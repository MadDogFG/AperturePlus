using AperturePlus.PortfolioService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Commands
{
    public class AddPhotoToGalleryCommand:IRequest<List<Guid>>
    {
        public Guid UserId { get; private set; }
        public Guid GalleryId { get; private set; }
        public List<FileToUpload> Files { get; private set; }

        public AddPhotoToGalleryCommand(Guid userId, Guid galleryId, List<FileToUpload> files)
        {
            UserId = userId;
            GalleryId = galleryId;
            Files = files;
        }
    }
}
