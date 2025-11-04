using AperturePlus.ChatService.Application.Commands;
using AperturePlus.ChatService.Application.Interfaces;
using AperturePlus.ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AperturePlus.ChatService.Api.Hubs
{
    [Authorize] // 确保只有登录用户才能连接
    public class ChatHub : Hub
    {
        private readonly IMediator mediator;
        private readonly IConversationRepository conversationRepository;
        private readonly IUserSummaryRepository userSummaryRepository;

        public ChatHub(IMediator mediator, IConversationRepository conversationRepository, IUserSummaryRepository userSummaryRepository)
        {
            this.mediator = mediator;
            this.conversationRepository = conversationRepository;
            this.userSummaryRepository = userSummaryRepository;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        // 当用户连接时，将他们加入到他们所有的会话"组"中
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var conversations = await conversationRepository.GetConversationsForUserAsync(userId, CancellationToken.None);
            foreach (var convo in conversations)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, convo.ConversationId.ToString());
            }
            await base.OnConnectedAsync();
        }

        // 供客户端调用，用于发起聊天
        public async Task<Conversation> StartChat(Guid recipientId)
        {
            var senderId = GetUserId();
            var conversation = await conversationRepository.FindByParticipantsAsync(senderId, recipientId, CancellationToken.None);

            if (conversation == null)
            {
                conversation = Conversation.Create(senderId, recipientId);
                await conversationRepository.AddAsync(conversation, CancellationToken.None);
            }

            // 将当前连接(发送者)加入组
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.ConversationId.ToString());

            // (可选) 检查接收者是否在线，如果在线，也将其连接加入组
            // 这需要一个更复杂的用户在线状态跟踪系统，暂时省略

            return conversation;
        }

        // 供客户端调用，用于发送消息
        public async Task SendMessage(Guid conversationId, string content)
        {
            var senderId = GetUserId();

            // 1. 保存消息到数据库
            var message = await mediator.Send(new SendMessageCommand(conversationId, senderId, content));

            // 2. (可选) 丰富消息数据，以便客户端显示
            var sender = await userSummaryRepository.GetByIdAsync(senderId, CancellationToken.None);
            var messageDto = new
            {
                message.MessageId,
                message.SenderId,
                SenderName = sender?.UserName ?? "未知用户",
                message.Content,
                message.Timestamp
            };

            // 3. 将消息广播给组内的所有连接（包括发送者自己）
            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", messageDto);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // (可选) 在此处理用户离线逻辑
            await base.OnDisconnectedAsync(exception);
        }
    }
}