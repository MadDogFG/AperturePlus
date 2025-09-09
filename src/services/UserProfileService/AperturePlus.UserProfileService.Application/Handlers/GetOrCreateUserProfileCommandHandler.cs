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
    public class GetOrCreateUserProfileCommandHandler : IRequestHandler<GetOrCreateUserProfileCommand, UserProfileDto>
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;

        public GetOrCreateUserProfileCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto> Handle(GetOrCreateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userProfile == null) 
            {
                userProfile = UserProfile.CreateUserProfile(request.UserId, request.UserName);
                await userProfileRepository.AddUserProfileAsync(userProfile, cancellationToken);
                int result = await unitOfWork.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return UserProfileDto.Success(userProfile);
                }
                else
                {
                    return UserProfileDto.Failure();
                }
            }
            return UserProfileDto.Success(userProfile);
        }
    }
}
