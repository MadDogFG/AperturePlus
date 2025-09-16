using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Commands
{
    public class DeleteGalleryCommand:IRequest<bool>
    {
        public Guid GalleryId { get; private set; }
        public Guid UserId {  get; private set; }

        public DeleteGalleryCommand(Guid galleryId, Guid userId)
        {
            GalleryId = galleryId;
            UserId = userId;
        }
    }
}
