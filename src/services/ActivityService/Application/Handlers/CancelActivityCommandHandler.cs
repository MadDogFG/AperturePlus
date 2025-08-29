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
    public class CancelActivityCommandHandler : IRequestHandler<CancelActivityCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;
        public CancelActivityCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CancelActivityCommand request, CancellationToken cancellationToken)
        {
            Activity? activity = await activityRepository.GetByIdAsync(request.ActivityId, cancellationToken);
            if (activity == null) 
            {
                return false;
            }
            if (request.UserId != activity.PostedByUserId)
            {
                return false;
            }
            try
            {
                activity.CancelActivity();
            }
            catch(InvalidOperationException)
            {
                return false;
            }
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
