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
        public Guid UserId { get; private set; }
        public RoleType Role { get; private set; }//参与者角色
        public ParticipantStatus Status { get; private set; } = ParticipantStatus.Pending;//申请状态，默认Pending
        public DateTime ApliedAt { get; private set; } = DateTime.UtcNow;//申请时间

        public Participant(Guid userId, RoleType role)
        {
            UserId = userId;
            Role = role;
        }

        public void Approve()
        {
            if (Status == ParticipantStatus.Approved)
                throw new InvalidOperationException("已通过该用户的参与资格");
            Status = ParticipantStatus.Approved;
        }

        public void Reject()
        {
            if (Status == ParticipantStatus.Rejected)
                throw new InvalidOperationException("已拒绝该用户的参与资格");
            Status = ParticipantStatus.Rejected;
        }
    }
}
