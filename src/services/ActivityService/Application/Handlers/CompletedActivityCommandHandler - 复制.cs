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
    public class CompletedActivityCommandHandler : IRequestHandler<CompletedActivityCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;
        public CompletedActivityCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CompletedActivityCommand request, CancellationToken cancellationToken)
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
                activity.CompletedActivity();
            }
            catch(InvalidOperationException)
            {
                return false;
            }
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            if(result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
