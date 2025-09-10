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
    public class GetByUserIdQueryHandler : IRequestHandler<GetByUserIdQuery, List<ActivityDto>>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserSummaryRepository userSummaryRepository;

        public GetByUserIdQueryHandler(IActivityRepository activityRepository, IUserSummaryRepository userSummaryRepository)
        {
            this.activityRepository = activityRepository;
            this.userSummaryRepository = userSummaryRepository;
        }

        public async Task<List<ActivityDto>> Handle(GetByUserIdQuery request, CancellationToken cancellationToken)
        {
            var activityList = await activityRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var postedByUserIds = activityList.Select(a => a.PostedByUserId).Distinct();
            var userSummaries = await userSummaryRepository.GetByIdsAsync(postedByUserIds, cancellationToken);
            var userSummariesDict = userSummaries.ToDictionary(u => u.UserId);
            var activityDtos = activityList.Select(activity =>
            {
                var userSummary = userSummariesDict.GetValueOrDefault(activity.PostedByUserId);
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
            return activityDtos;
        }
    }
}
