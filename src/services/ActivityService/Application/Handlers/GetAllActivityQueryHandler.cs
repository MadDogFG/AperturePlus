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
            var pagedActivities = await activityRepository.GetAllAsync(request.Page,request.PageSize,cancellationToken);
            var userIds = pagedActivities.Activities.Select(a => a.PostedByUserId).Distinct();
            var userSummaries = await userSummaryRepository.GetByIdsAsync(userIds, cancellationToken);
            var userSummariesDict = userSummaries.ToDictionary(u => u.UserId);
            var items = pagedActivities.Activities.Select(activity =>
            {
                var userSummary = userSummariesDict.GetValueOrDefault(activity.PostedByUserId);
                if(userSummary == null)
                {
                    throw new Exception($"无法找到用户ID为 {activity.PostedByUserId} 的用户信息");
                }
                return new ActivityDto(
                    activity.ActivityId,
                    activity.ActivityTitle,
                    activity.ActivityDescription,
                    activity.ActivityLocation,
                    activity.ActivityStartTime,
                    new PostedByUserDto(activity.PostedByUserId, userSummary.UserName),
                    activity.Status.ToString(),
                    activity.Fee,
                    activity.RoleRequirements,
                    activity.Participants
                );
            }).ToList();
            return new ActivityListResult(items, pagedActivities.HasMore);
        }
    }
}
