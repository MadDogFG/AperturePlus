using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Handlers
{
    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;
        public UpdateActivityCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await activityRepository.GetByIdAsync(request.ActivityId, cancellationToken);
            if (activity == null)
            {
                return false;
            }
            if (activity.PostedByUserId != request.UserId)
            {
                return false; //用户无权更新此活动
            }
            activity.UpdateActivity(
                request.ActivityTitle,
                request.ActivityDescription,
                request.ActivityLocation,
                request.ActivityStartTime
            );
            activityRepository.UpdateActivity(activity);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
