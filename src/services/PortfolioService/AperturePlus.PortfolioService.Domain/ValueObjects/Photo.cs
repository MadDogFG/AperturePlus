using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Domain.ValueObjects
{
    public record class Photo
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid PhotoId { get; init; }
        public string PhotoUrl { get; init; }
        public List<string> Tags { get; private set; } = new List<string>();//留着未来调用模型进行智能分类
        public DateTime UploadedAt { get; init; }

        private Photo(string photoUrl)
        {
            PhotoId = Guid.NewGuid();
            PhotoUrl = photoUrl;
            UploadedAt = DateTime.UtcNow;
        }

        public static Photo CreatePhoto(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                throw new ArgumentException("图片路径为空");
            }
            return new Photo(photoUrl);
        }
    }
}
