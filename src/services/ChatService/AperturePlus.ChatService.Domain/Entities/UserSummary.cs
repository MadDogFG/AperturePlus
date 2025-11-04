using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Domain.Entities
{
    public class UserSummary
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public string AvatarUrl { get; private set; }

        private UserSummary(Guid userId, string userName, string avatarUrl)
        {
            UserId = userId;
            UserName = userName;
            AvatarUrl = avatarUrl;
        }

        public static UserSummary Create(Guid userId, string userName, string avatarUrl)
        {
            return new UserSummary(userId, userName, avatarUrl);
        }

        public void Update(string newAvatarUrl)
        {
            AvatarUrl = newAvatarUrl;
        }
    }
}
