using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Domain.Entities
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ConversationId { get; private set; }

        // 存储参与者的ID，用于快速查询
        public List<Guid> ParticipantIds { get; private set; } = new List<Guid>();

        // 存储消息
        public List<Message> Messages { get; private set; } = new List<Message>();

        public DateTime LastUpdatedAt { get; private set; }

        private Conversation(List<Guid> participantIds)
        {
            ConversationId = Guid.NewGuid();
            ParticipantIds = participantIds;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public static Conversation Create(Guid userOneId, Guid userTwoId)
        {
            var participants = new List<Guid> { userOneId, userTwoId };
            return new Conversation(participants);
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
            LastUpdatedAt = message.Timestamp;
        }
    }
}
