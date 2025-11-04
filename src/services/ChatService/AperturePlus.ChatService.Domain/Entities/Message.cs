using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Domain.Entities
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid MessageId { get; private set; }
        [BsonRepresentation(BsonType.String)]
        public Guid SenderId { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        private Message(Guid senderId, string content)
        {
            MessageId = Guid.NewGuid();
            SenderId = senderId;
            Content = content;
            Timestamp = DateTime.UtcNow;
        }

        public static Message Create(Guid senderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("消息内容不能为空");
            }
            return new Message(senderId, content);
        }
    }
}
