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
    public class GetAllActivityQueryHandler : IRequestHandler<GetAllActivityQuery, ActivityListResult>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserSummaryRepository userSummaryRepository;

        public GetAllActivityQueryHandler(IActivityRepository activityRepository,IUserSummaryRepository userSummaryRepository)
        {
            this.activityRepository = activityRepository;
            this.userSummaryRepository = userSummaryRepository;
        }

        public async Task<ActivityListResult> Handle(GetAllActivityQuery request, CancellationToken cancellationToken)
        {
            //获取分页数据
            var pagedActivities = await activityRepository.GetPagedAsync(request.Page,request.PageSize,cancellationToken);
            var items = new List<ActivityDto>();
            foreach (var activity in pagedActivities.Activities)
            {
                var userSummary = await userSummaryRepository.GetByIdAsync(activity.PostedByUserId, cancellationToken);

                items.Add(new ActivityDto(
                    activity.ActivityId,
                    activity.ActivityTitle,
                    activity.ActivityDescription,
                    activity.ActivityLocation,
                    activity.ActivityStartTime,
                    new PostedByUserDto(userSummary.UserId, userSummary.UserName),
                    activity.Status.ToString(),
                    activity.Fee,
                    activity.RoleRequirements,
                    activity.Participants
                ));
            }
            return new ActivityListResult(items, pagedActivities.HasMore);
        }
    }
}
