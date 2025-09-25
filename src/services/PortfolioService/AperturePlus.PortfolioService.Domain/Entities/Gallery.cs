using AperturePlus.PortfolioService.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Domain.Entities
{
    public class Gallery
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid GalleryId { get; private set; }
        public string GalleryName { get; private set; }
        public string? CoverPhotoUrl { get; private set; }//封面图
        [BsonElement("Photos")]
        [BsonIgnoreIfNull]
        public List<Photo> Photos { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Gallery()
        {
            Photos = new List<Photo>();
        }

        private Gallery(string galleryName) : this()
        {
            GalleryId = Guid.NewGuid();
            GalleryName = galleryName;
            CreatedAt = DateTime.UtcNow;
        }

        public static Gallery CreateGallery(string galleryName)
        {
            if (string.IsNullOrEmpty(galleryName))
            {
                throw new ArgumentException("相册名称不能为空");
            }
            return new Gallery(galleryName);
        }

        public void AddPhoto(Photo photo)
        {
            if (photo == null)
            {
                throw new ArgumentNullException("图片不能为空");
            }
            Photos.Add(photo);
            //如果是第一张图，设置为封面图
            if (Photos.Count == 1)
            {
                CoverPhotoUrl = photo.PhotoUrl;
            }
        }

        public void RemovePhoto(Guid photoId)
        {
            var photoToRemove = Photos.FirstOrDefault(p => p.PhotoId == photoId);
            if (photoToRemove != null)
            {
                bool wasCover = CoverPhotoUrl == photoToRemove.PhotoUrl;

                Photos.Remove(photoToRemove);

                // 如果被删除的是封面图，我们需要更新它
                if (wasCover)
                {
                    // 将新的封面设置为当前照片列表的第一张，或者如果没有照片了就设为null
                    CoverPhotoUrl = Photos.FirstOrDefault()?.PhotoUrl;
                }
            }
        }

        public void SetCoverPhoto(Guid photoId)
        {
            var photoUrl = Photos.Where(p=>p.PhotoId==photoId).Select(p => p.PhotoUrl).FirstOrDefault();
            if (string.IsNullOrEmpty(photoUrl))
            {
                throw new ArgumentException("图片不存在");
            }
            CoverPhotoUrl = photoUrl;
        }

        public void RenameGallery(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("相册名称不能为空");
            }
            GalleryName = newName;
        }
    }
}
