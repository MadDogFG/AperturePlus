using AperturePlus.PortfolioService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Domain.Entities
{
    public class Portfolio
    {
        public Guid PortfolioId { get; private set; }
        public Guid UserId { get; private set; } // 这是档案所有者的ID

        private readonly List<Gallery> _galleries = new();
        public IReadOnlyCollection<Gallery> Galleries => _galleries.AsReadOnly();

        private Portfolio(Guid userId)
        {
            PortfolioId = Guid.NewGuid();
            UserId = userId;
        }

        public static Portfolio CreatePortfolio(Guid userId)
        {
            return new Portfolio(userId);
        }

        public Guid CreateNewGallery(string galleryName)
        {
            var newGallery = Gallery.CreateGallery(galleryName);
            _galleries.Add(newGallery);
            return newGallery.GalleryId;
        }

        public void DeleteGallery(Guid galleryId)
        {
            var rmGallery = _galleries.FirstOrDefault(g => g.GalleryId == galleryId);
            if (rmGallery == null)
            {
                throw new ArgumentException("相册不存在");
            }
            _galleries.Remove(rmGallery);
        }
    }
}
