using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Handlers
{
    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, bool>
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDistributedCache cache;
        private readonly ILogger<UpdateUserRolesCommandHandler> logger;

        // 5. 更新构造函数
        public UpdateUserRolesCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork, IDistributedCache cache, ILogger<UpdateUserRolesCommandHandler> logger)
        {
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<bool> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userProfile == null)
            {
                logger.LogWarning("尝试为一个不存在的用户资料更新角色, UserId: {UserId}", request.UserId);
                return false;
            }

            userProfile.UpdateRoles(request.Roles);
            userProfileRepository.UpdateUserProfile(userProfile);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                try
                {
                    string cacheKey = $"user:{request.UserId}";
                    await cache.RemoveAsync(cacheKey, cancellationToken);
                    logger.LogInformation("因角色变更导致缓存失效成功, CacheKey: {CacheKey}", cacheKey);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Redis 缓存删除失败, CacheKey: user:{UserId}", request.UserId);
                }
            }
            return result > 0;
        }
    }
}