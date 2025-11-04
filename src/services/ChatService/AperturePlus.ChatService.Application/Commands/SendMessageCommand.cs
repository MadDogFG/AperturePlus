using AperturePlus.ChatService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ChatService.Application.Commands
{
    public record SendMessageCommand(Guid ConversationId,Guid SenderId,string Content):IRequest<Message>;
}
