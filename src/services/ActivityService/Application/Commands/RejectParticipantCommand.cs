using AperturePlus.ActivityService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Commands
{
    public class RejectParticipantCommand:IRequest<bool>
    {
        public Guid ActivityId { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public Guid ApplicantUserId { get; private set; }

        public RejectParticipantCommand(Guid activityId, Guid ownerUserId, Guid applicantUserId)
        {
            ActivityId = activityId;
            OwnerUserId = ownerUserId;
            ApplicantUserId = applicantUserId;
        }
    }
}
