using AperturePlus.ChatService.Application.Interfaces;
using AperturePlus.ChatService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AperturePlus.ChatService.Api.Controllers
{
    [Authorize]
    [Route("api/conversations")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConversationRepository conversationRepo;
        private readonly IUserSummaryRepository userSummaryRepo;

        public ChatController(IConversationRepository conversationRepo, IUserSummaryRepository userSummaryRepo)
        {
            this.conversationRepo = conversationRepo;
            this.userSummaryRepo = userSummaryRepo;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // DTO 定义 (为方便起见，放在 Controller 中)
        public record ChatUserDto(Guid UserId, string UserName, string AvatarUrl);
        public record ConversationListDto(Guid ConversationId, ChatUserDto Participant, Message? LastMessage, DateTime LastUpdatedAt);
        public record ConversationDto(Guid ConversationId, List<ChatUserDto> Participants, List<Message> Messages, DateTime LastUpdatedAt);


        [HttpGet]
        public async Task<IActionResult> GetConversations(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var conversations = await conversationRepo.GetConversationsForUserAsync(userId, cancellationToken);

            var dtos = new List<ConversationListDto>();
            foreach (var convo in conversations)
            {
                var otherUserId = convo.ParticipantIds.FirstOrDefault(id => id != userId);
                var userSummary = await userSummaryRepo.GetByIdAsync(otherUserId, cancellationToken);

                dtos.Add(new ConversationListDto(
                    convo.ConversationId,
                    new ChatUserDto(otherUserId, userSummary?.UserName ?? "未知用户", userSummary?.AvatarUrl ?? ""),
                    convo.Messages.LastOrDefault(),
                    convo.LastUpdatedAt
                ));
            }
            return Ok(dtos.OrderByDescending(d => d.LastUpdatedAt));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationById(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var convo = await conversationRepo.GetByIdAsync(id, cancellationToken);
            if (convo == null || !convo.ParticipantIds.Contains(userId))
            {
                return NotFound();
            }

            var participantDtos = new List<ChatUserDto>();
            foreach (var pid in convo.ParticipantIds)
            {
                var userSummary = await userSummaryRepo.GetByIdAsync(pid, cancellationToken);
                participantDtos.Add(new ChatUserDto(pid, userSummary?.UserName ?? "未知", userSummary?.AvatarUrl ?? ""));
            }

            return Ok(new ConversationDto(convo.ConversationId, participantDtos, convo.Messages, convo.LastUpdatedAt));
        }

        [HttpGet("with/{recipientId}")]
        public async Task<IActionResult> GetOrCreateConversation(Guid recipientId, CancellationToken cancellationToken)
        {
            var senderId = GetUserId();
            if (senderId == recipientId)
            {
                return BadRequest(new { Message = "不能和自己发起聊天" });
            }

            var convo = await conversationRepo.FindByParticipantsAsync(senderId, recipientId, cancellationToken);
            if (convo == null)
            {
                convo = Conversation.Create(senderId, recipientId);
                await conversationRepo.AddAsync(convo, cancellationToken);
            }

            // 复用 GetConversationById 的逻辑来返回完整 DTO
            return await GetConversationById(convo.ConversationId, cancellationToken);
        }
    }
}
