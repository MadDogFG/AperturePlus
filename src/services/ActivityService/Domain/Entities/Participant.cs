using AperturePlus.ActivityService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Domain.Entities
{
    public record class Participant
    {
        public Guid UserId { get; private init; }
        public RoleType Role { get; private init; }//参与者角色
        public ParticipantStatus Status { get; private set; } = ParticipantStatus.Pending;//申请状态，默认Pending
        public DateTime ApliedAt { get; private init; } = DateTime.UtcNow;//申请时间

        public Participant(Guid userId, RoleType role)
        {
            UserId = userId;
            Role = role;
        }

        public void Approve()
        {
            if (Status != ParticipantStatus.Pending)
                throw new InvalidOperationException("只能批准处于待处理状态的参与者");
            Status = ParticipantStatus.Approved;
        }

        public void Reject()
        {
            if (Status != ParticipantStatus.Pending)
                throw new InvalidOperationException("只能拒绝处于待处理状态的参与者");
            Status = ParticipantStatus.Rejected;
        }
    }
}
