using AperturePlus.UserProfileService.Application.Interfaces;
using AperturePlus.UserProfileService.Domain.Entities;
using AperturePlus.UserProfileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly UserProfileDbContext dbContext;

        public UserProfileRepository(UserProfileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddUserProfileAsync(UserProfile userProfile, CancellationToken cancellationToken)
        {
            await dbContext.UserProfiles.AddAsync(userProfile);
        }

        public async Task<UserProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.UserProfiles.FirstOrDefaultAsync(u=>u.UserId == userId,cancellationToken);
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            dbContext.UserProfiles.Update(userProfile);
        }
    }
}
