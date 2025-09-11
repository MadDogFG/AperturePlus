using AperturePlus.PortfolioService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Domain.Entities
{
    public class Gallery
    {
        public Guid GalleryId { get; private set; }
        public string GalleryName { get; private set; }
        public string? CoverPhotoUrl { get; private set; }//封面图
        private readonly List<Photo> _photos = new();
        public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();//只读集合，防止外部能直接通过属性添加或者删除图片
        public DateTime CreatedAt { get; private set; }

        private Gallery(string galleryName)
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
            _photos.Add(photo);
            //如果是第一张图，设置为封面图
            if (_photos.Count == 1)
            {
                CoverPhotoUrl = photo.PhotoUrl;
            }
        }

        public void RemovePhoto(Guid photoId)
        {
            var photo = _photos.FirstOrDefault(p=>p.PhotoId == photoId);
            if (photo == null) 
            {
                throw new ArgumentException("图片不存在");
            }
            _photos.Remove(photo);
        }

        public void SetCoverPhoto(Guid photoId)
        {
            var photoUrl = _photos.Where(p=>p.PhotoId==photoId).Select(p => p.PhotoUrl).FirstOrDefault();
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
