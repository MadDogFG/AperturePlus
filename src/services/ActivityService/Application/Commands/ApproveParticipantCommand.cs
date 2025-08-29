using AperturePlus.ActivityService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Commands
{
    public record class ApproveParticipantCommand : IRequest<bool>
    {
        public Guid ActivityId { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public Guid ApplicantUserId { get; private set; }
        public RoleType Role { get; private set; }

        public ApproveParticipantCommand(Guid activityId, Guid ownerUserId, Guid applicantUserId, RoleType role)
        {
            ActivityId = activityId;
            OwnerUserId = ownerUserId;
            ApplicantUserId = applicantUserId;
            Role = role;
        }
    }
}
