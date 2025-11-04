using AperturePlus.ChatService.Application.Interfaces;
using AperturePlus.ChatService.Domain.Entities;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IMongoCollection<Conversation> collection;

        public ConversationRepository(IMongoCollection<Conversation> collection)
        {
            this.collection = collection;
        }

        public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken)
        {
            await collection.InsertOneAsync(conversation, null, cancellationToken);
        }

        public async Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken)
        {
            return await collection.Find(c => c.ConversationId == conversationId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Conversation?> FindByParticipantsAsync(Guid userOneId, Guid userTwoId, CancellationToken cancellationToken)
        {
            return await collection.Find(c => c.ParticipantIds.Contains(userOneId) && c.ParticipantIds.Contains(userTwoId))
                             .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Conversation>> GetConversationsForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await collection.Find(c => c.ParticipantIds.Contains(userId))
                             .SortByDescending(c => c.LastUpdatedAt)
                             .ToListAsync(cancellationToken);
        }
        public async Task<bool> UpdateAsync(Conversation conversation, CancellationToken cancellationToken)
        {
            var result = await collection.ReplaceOneAsync(c => c.ConversationId == conversation.ConversationId, conversation, cancellationToken: cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
    public class UserSummaryRepository : IUserSummaryRepository
    {
        private readonly IMongoCollection<UserSummary> collection;

        public UserSummaryRepository(IMongoCollection<UserSummary> collection)
        {
            this.collection = collection;
        }
        public async Task AddAsync(UserSummary userSummary, CancellationToken cancellationToken)
        {
            await collection.InsertOneAsync(userSummary, null, cancellationToken);
        }

        public async Task<UserSummary?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await collection.Find(c => c.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(UserSummary userSummary, CancellationToken cancellationToken)
        {
            var result = await collection.ReplaceOneAsync(c => c.UserId == userSummary.UserId, userSummary, cancellationToken: cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
