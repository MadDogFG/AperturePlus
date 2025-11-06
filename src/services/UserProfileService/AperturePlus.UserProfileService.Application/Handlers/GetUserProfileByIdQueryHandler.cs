using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.DTOs;
using AperturePlus.UserProfileService.Application.Interfaces;
using AperturePlus.UserProfileService.Application.Queries;
using AperturePlus.UserProfileService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Handlers
{
    public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto?>
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IDistributedCache cache;
        private readonly ILogger<GetUserProfileByIdQueryHandler> logger;

        public GetUserProfileByIdQueryHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork,IDistributedCache cache, ILogger<GetUserProfileByIdQueryHandler> logger)
        {
            this.userProfileRepository = userProfileRepository;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<UserProfileDto?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
        {
            //定义缓存键
            string cacheKey = $"user:{request.UserId}";
            UserProfileDto? userProfileDto = null;
            string? cachedProfileJson = null;
            try
            {
                //尝试从缓存中获取
                cachedProfileJson = await cache.GetStringAsync(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis 缓存读取失败, CacheKey: {CacheKey}", cacheKey);
            }
            if (!string.IsNullOrEmpty(cachedProfileJson))
            {
                //缓存命中
                logger.LogInformation("缓存命中, CacheKey: {CacheKey}", cacheKey);
                userProfileDto = JsonSerializer.Deserialize<UserProfileDto>(cachedProfileJson);
                return userProfileDto;
            }
            //缓存未命中
            logger.LogInformation("缓存未命中, CacheKey: {CacheKey}", cacheKey);
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userProfile != null)
            {
                userProfileDto = new UserProfileDto(userProfile.UserId, userProfile.UserName, userProfile.Bio, userProfile.AvatarUrl, userProfile.Roles);
                //将查询结果写入缓存中
                try
                {
                    var profileToCacheJson = JsonSerializer.Serialize(userProfileDto);
                    var cacheOptions = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromHours(1)) //1小时内无访问则过期
                        .SetAbsoluteExpiration(TimeSpan.FromDays(1)); //最长缓存1天

                    await cache.SetStringAsync(cacheKey, profileToCacheJson, cacheOptions, cancellationToken);
                    logger.LogInformation("已将结果写入缓存, CacheKey: {CacheKey}", cacheKey);
                }
                catch (Exception ex)
                {
                    // 如果Redis挂了，记录错误但不影响主流程
                    logger.LogError(ex, "Redis 缓存写入失败, CacheKey: {CacheKey}", cacheKey);
                }
                return userProfileDto;
            }
            return null;
        }
    }
}
