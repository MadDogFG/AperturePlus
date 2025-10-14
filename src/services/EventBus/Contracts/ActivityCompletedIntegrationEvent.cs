using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public record class ActivityCompletedIntegrationEvent
    {
        public Guid ActivityId { get; init; }
        public string ActivityTitle { get; init; }
        public List<ParticipantDto> Participants { get; init; } = new();

        public ActivityCompletedIntegrationEvent(Guid activityId, string activityTitle,List<ParticipantDto> participants)
        {
            ActivityId = activityId;
            ActivityTitle = activityTitle;
            Participants = participants;
        }

        public record ParticipantDto
        {
            public Guid UserId { get; init; }
            public string UserName { get; init; }
            public string Role { get; init; } // 使用string以保证事件的通用性

            public ParticipantDto(Guid userId, string userName ,string role)
            {
                UserId = userId;
                UserName = userName;
                Role = role;
            }
        }
    }
}
