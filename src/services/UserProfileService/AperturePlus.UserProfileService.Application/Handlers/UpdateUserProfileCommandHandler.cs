using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.Interfaces;
using Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Handlers
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessageBus messageBus;

        public UpdateUserProfileCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
            this.messageBus = messageBus;
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId,cancellationToken);
            if (userProfile == null) 
            {
                return false;
            }
            userProfile.UpdateUserProfile(request.Bio, request.AvatarUrl);
            userProfileRepository.UpdateUserProfile(userProfile);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (result > 0)
            {
                var profileUpdatedEvent = new UserProfileUpdatedIntegrationEvent(
                    userProfile.UserId,
                    userProfile.UserName,
                    userProfile.AvatarUrl
                );
                await messageBus.Publish("user_events", "user.profile.updated", profileUpdatedEvent);
            }

            return result > 0;
        }
    }
}
