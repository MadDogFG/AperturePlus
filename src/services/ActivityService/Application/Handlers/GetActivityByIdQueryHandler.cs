using AperturePlus.ActivityService.Application.DTOs;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Application.Queries;
using AperturePlus.ActivityService.Domain.ValueObjects;
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
        private readonly IUserSummaryRepository userSummaryRepository;
        public GetActivityByIdQueryHandler(IActivityRepository activityRepository, IUserSummaryRepository userSummaryRepository)
        {
            this.activityRepository = activityRepository;
            this.userSummaryRepository = userSummaryRepository;
        }
        public async Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
        {
            var activity = await activityRepository.GetByIdAsync(request.ActivityId, cancellationToken);
            if (activity == null)
            {
                return null;
            }
            //计算人数
            var totalRequired = activity.RoleRequirements.Sum(r => r.Quantity);
            var approvedCount = activity.Participants.Count(p => p.Status == ParticipantStatus.Approved);
            var pendingCount = activity.Participants.Count(p => p.Status == ParticipantStatus.Pending);

            var userSummary = await userSummaryRepository.GetByIdAsync(activity.PostedByUserId, cancellationToken);
            return new ActivityDto
            (
                activity.ActivityId,
                activity.ActivityTitle,
                activity.ActivityDescription,
                activity.ActivityLocation,
                activity.ActivityStartTime,
                new PostedByUserDto(userSummary.UserId, userSummary.UserName),//未来需要用集成事件来获取用户信息
                activity.Status.ToString(),
                activity.Fee,
                activity.RoleRequirements,
                activity.Participants,
                totalRequired,
                approvedCount,
                pendingCount

            );
        }
    }
}
