using AperturePlus.UserProfileService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Interfaces
{
    public interface IUserProfileRepository
    {
        Task AddUserProfileAsync(UserProfile userProfile, CancellationToken cancellationToken);
        void UpdateUserProfile(UserProfile userProfile);
        Task<UserProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
