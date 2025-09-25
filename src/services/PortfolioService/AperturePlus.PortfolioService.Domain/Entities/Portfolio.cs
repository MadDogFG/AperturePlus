using AperturePlus.PortfolioService.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Domain.Entities
{
    public class Portfolio
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid PortfolioId { get; private set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; private set; } // 这是档案所有者的ID

        [BsonElement("Galleries")] // 明确字段名
        [BsonIgnoreIfNull] // 如果Galleries为null则不序列化，并确保反序列化时不会覆盖已初始化的空列表
        private List<Gallery> _galleries;
        public IReadOnlyCollection<Gallery> Galleries => _galleries.AsReadOnly();

        // 为BSON反序列化器提供的构造函数
        private Portfolio()
        {
            // 在这里显式初始化，确保列表永远不会为null
            _galleries = new List<Gallery>();
        }
        private Portfolio(Guid userId):this()
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
            if (_galleries.Any(g => g.GalleryName.Equals(galleryName, StringComparison.OrdinalIgnoreCase)))
            {
                // 如果已存在同名相册（忽略大小写），则抛出异常
                throw new InvalidOperationException($"名为 '{galleryName}' 的相册已存在。");
            }
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
