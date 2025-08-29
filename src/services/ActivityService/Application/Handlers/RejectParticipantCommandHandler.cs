using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Handlers
{
    public class RejectParticipantCommandHandler:IRequestHandler<RejectParticipantCommand,bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;

        public RejectParticipantCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RejectParticipantCommand request, CancellationToken cancellationToken)
        {
            Activity? actvity = await activityRepository.GetByIdAsync(request.ActivityId, cancellationToken);
            if (actvity == null)
            {
                return false;
            }
            if (actvity.PostedByUserId != request.OwnerUserId)
            {
                return false;
            }
            try
            {
                actvity.RejectParticipant(request.ApplicantUserId);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            activityRepository.UpdateActivity(actvity);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
