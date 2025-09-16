using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Commands
{
    public class DeletePhotoCommand : IRequest<bool>
    {
        public Guid UserId { get; private set; }
        public Guid GalleryId { get; private set; }
        public List<Guid> PhotoIds { get; private set; }

        public DeletePhotoCommand(Guid userId, Guid galleryId, List<Guid> photoIds)
        {
            UserId = userId;
            GalleryId = galleryId;
            PhotoIds = photoIds;
        }
    }
}
