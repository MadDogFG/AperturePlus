using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.Interfaces;
using Contracts;
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
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessageBus messageBus;
        private readonly IDistributedCache cache;
        private readonly ILogger<UpdateUserProfileCommandHandler> logger;

        public UpdateUserProfileCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork, IMessageBus messageBus,IDistributedCache cache, ILogger<UpdateUserProfileCommandHandler> logger)
        {                                                                                                          
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
            this.messageBus = messageBus;
            this.cache = cache;
            this.logger = logger;
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
                //在数据库更新成功后，立即失效缓存
                try
                {
                    string cacheKey = $"user:{request.UserId}";
                    await cache.RemoveAsync(cacheKey, cancellationToken);
                    logger.LogInformation("缓存失效成功, CacheKey: {CacheKey}", cacheKey);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Redis 缓存删除失败, CacheKey: user:{UserId}", request.UserId);
                }
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
