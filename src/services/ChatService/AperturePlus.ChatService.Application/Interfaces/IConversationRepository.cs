using AperturePlus.ChatService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Application.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken);
        Task<Conversation?> FindByParticipantsAsync(Guid userOneId, Guid userTwoId, CancellationToken cancellationToken);
        Task<IEnumerable<Conversation>> GetConversationsForUserAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(Conversation conversation, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Conversation conversation, CancellationToken cancellationToken);
    }

    public interface IUserSummaryRepository
    {
        Task AddAsync(UserSummary userSummary, CancellationToken cancellationToken);
        Task<UserSummary?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(UserSummary userSummary, CancellationToken cancellationToken);
    }
}
