using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Domain.Entities;
using Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Contracts.ActivityCompletedIntegrationEvent;

namespace AperturePlus.ActivityService.Application.Handlers
{
    public class CompletedActivityCommandHandler : IRequestHandler<CompletedActivityCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserSummaryRepository userSummaryRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessageBus messageBus;
        public CompletedActivityCommandHandler(IActivityRepository activityRepository, IUserSummaryRepository userSummaryRepository ,IUnitOfWork unitOfWork,IMessageBus messageBus)
        {
            this.activityRepository = activityRepository;
            this.userSummaryRepository = userSummaryRepository;
            this.unitOfWork = unitOfWork;
            this.messageBus = messageBus;
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
            var activityParticipants = new List<ParticipantDto>();
            foreach (var item in activity.Participants)
            {
                string userName = (await userSummaryRepository.GetByIdAsync(item.UserId, cancellationToken)).UserName;
                activityParticipants.Add(new ParticipantDto(item.UserId, userName, item.Role.ToString()));
            }
            var activityCompletedIntegrationEvent = new ActivityCompletedIntegrationEvent(activity.ActivityId,activity.ActivityTitle,activityParticipants);
            await messageBus.Publish("activity_events", "activity.completed", activityCompletedIntegrationEvent);
            return result > 0;
        }
    }
}
