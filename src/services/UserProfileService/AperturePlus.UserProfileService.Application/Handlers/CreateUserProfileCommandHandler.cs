using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.DTOs;
using AperturePlus.UserProfileService.Application.Interfaces;
using AperturePlus.UserProfileService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Handlers
{
    public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, bool>
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateUserProfileCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var existingProfile = await userProfileRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (existingProfile!=null)
            {
                return true;
            }
            var userProfile = UserProfile.CreateUserProfile(request.UserId, request.UserName);
            await userProfileRepository.AddUserProfileAsync(userProfile, cancellationToken);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
