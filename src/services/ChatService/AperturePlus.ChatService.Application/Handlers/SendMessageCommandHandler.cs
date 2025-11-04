using AperturePlus.ChatService.Application.Commands;
using AperturePlus.ChatService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AperturePlus.ChatService.Domain.Entities;

namespace AperturePlus.ChatService.Application.Handlers
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Message>
    {
        private readonly IConversationRepository conversationRepository;
        public SendMessageCommandHandler(IConversationRepository conversationRepository)
        {
            this.conversationRepository = conversationRepository;
        }
        public async Task<Message> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                throw new InvalidOperationException("会话不存在");
            }
            if (!conversation.ParticipantIds.Contains(request.SenderId))
            {
                throw new InvalidOperationException("用户不在此会话中");
            }

            var message = Message.Create(request.SenderId, request.Content);
            conversation.AddMessage(message);

            await conversationRepository.UpdateAsync(conversation, cancellationToken);

            return message;
        }
    }
}
