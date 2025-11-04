using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Application.DTOs
{
    public class MessageDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid MessageId { get; private set; }
        [BsonRepresentation(BsonType.String)]
        public Guid SenderId { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        public MessageDto(Guid messageId,Guid senderId, string content,DateTime timestamp)
        {
            MessageId = messageId;
            SenderId = senderId;
            Content = content;
            Timestamp = timestamp;
        }
    }
}
