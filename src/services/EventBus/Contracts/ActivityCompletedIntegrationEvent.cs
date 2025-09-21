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
        public List<ParticipantDto> Participants { get; init; } = new();

        public ActivityCompletedIntegrationEvent(Guid ActivityId, List<ParticipantDto> Participants)
        {
            this.ActivityId = ActivityId;
            this.Participants = Participants;
        }

        public record ParticipantDto
        {
            public Guid UserId { get; init; }
            public string Role { get; init; } // 使用string以保证事件的通用性

            public ParticipantDto(Guid userId, string role)
            {
                UserId = userId;
                Role = role;
            }
        }
    }
}
