using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.DTOs;
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
    public class CreateActivityCommandHandler:IRequestHandler<CreateActivityCommand,CreateActivityResult>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateActivityCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<CreateActivityResult> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
        {
            Activity activity = Activity.ActivityCreate(
                    request.ActivityTitle,
                    request.ActivityDescription,
                    request.ActivityLocation,
                    request.ActivityStartTime,
                    request.PostedByUserId
            );
            await activityRepository.AddActivityAsync(activity,cancellationToken);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return CreateActivityResult.Success(activity.ActivityId);
            }
            else
            {
                return CreateActivityResult.Failure();
            }
        }
    }
}
