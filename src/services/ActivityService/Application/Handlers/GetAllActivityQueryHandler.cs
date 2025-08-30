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
    public class GetAllActivityQueryHandler : IRequestHandler<GetAllActivityQuery, IEnumerable<ActivityDto>>
    {
        private readonly IActivityRepository activityRepository;

        public GetAllActivityQueryHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository;
        }

        public async Task<IEnumerable<ActivityDto>> Handle(GetAllActivityQuery request, CancellationToken cancellationToken)
        {
            var activityList = await activityRepository.GetAllAsync(cancellationToken);
            return activityList.Select(activity => new ActivityDto
            (
                activity.ActivityId,
                activity.ActivityTitle,
                activity.ActivityDescription,
                activity.ActivityLocation,
                activity.ActivityStartTime,
                new PostedByUserDto(activity.PostedByUserId,"占位"),//未来需要用集成事件来获取用户信息
                activity.Status.ToString(),
                activity.Fee,
                activity.RoleRequirements,
                activity.Participants
            ));
        }
    }
}
