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
    public class ApproveParticipantCommandHandler : IRequestHandler<ApproveParticipantCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;

        public ApproveParticipantCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ApproveParticipantCommand request, CancellationToken cancellationToken)
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
                actvity.ApproveParticipant(request.ApplicantUserId,request.Role);
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
