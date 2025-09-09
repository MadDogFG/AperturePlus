using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.DTOs;
using AperturePlus.UserProfileService.Application.Interfaces;
using AperturePlus.UserProfileService.Application.Queries;
using AperturePlus.UserProfileService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Handlers
{
    public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto?>
    {
        private readonly IUserProfileRepository userProfileRepository;

        public GetUserProfileByIdQueryHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfileDto?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userProfile != null)
            {
                return new UserProfileDto(userProfile.UserId, userProfile.UserName, userProfile.Bio, userProfile.AvatarUrl);
            }
            return null;
        }
    }
}
