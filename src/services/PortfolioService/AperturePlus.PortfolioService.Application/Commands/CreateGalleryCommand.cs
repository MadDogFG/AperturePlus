using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Commands
{
    public class CreateGalleryCommand:IRequest<Guid>
    {
        public Guid UserId { get; private set; }
        public string GalleryName { get; private set; }

        public CreateGalleryCommand(Guid userId, string galleryName)
        {
            UserId = userId;
            GalleryName = galleryName;
        }
    }
}
