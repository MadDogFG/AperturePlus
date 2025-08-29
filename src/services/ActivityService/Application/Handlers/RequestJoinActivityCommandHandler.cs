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
    public class RequestJoinActivityCommandHandler : IRequestHandler<RequestJoinActivityCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;

        public RequestJoinActivityCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RequestJoinActivityCommand request, CancellationToken cancellationToken)
        {
            Activity? activity = await activityRepository.GetByIdAsync(request.ActivityId,cancellationToken);
            if (activity == null) 
            {
                return false;
            }
            try
            {
                activity.RequestJoinActivity(request.UserId, request.Role);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            activityRepository.UpdateActivity(activity);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
