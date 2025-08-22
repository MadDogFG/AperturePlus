using AperturePlus.ActivityService.Application.DTOs;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Application.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Handlers
{
    public class GetActivityByIdQueryHandler : IRequestHandler<GetActivityByIdQuery, ActivityDto?>
    {
        private readonly IActivityRepository activityRepository;
        public GetActivityByIdQueryHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository;
        }
        public async Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
        {
            var activity = await activityRepository.GetByIdAsync(request.ActivityId, cancellationToken);
            if (activity == null)
            {
                return null;
            }
            return new ActivityDto
            (
                activity.ActivityId,
                activity.ActivityTitle,
                activity.ActivityDescription,
                activity.ActivityLocation,
                activity.ActivityStartTime,
                new PostedByUserDto(activity.PostedByUserId, "占位"),//未来需要用集成事件来获取用户信息
                activity.Status.ToString()
            );
        }
    }
}
