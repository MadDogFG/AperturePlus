using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.Interfaces;
using AperturePlus.ActivityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Handlers
{
    public class RequestJoinActivityCommandHandler : IRequestHandler<RequestJoinActivityCommand, bool>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RequestJoinActivityCommandHandler> logger;

        public RequestJoinActivityCommandHandler(IActivityRepository activityRepository, IUnitOfWork unitOfWork, ILogger<RequestJoinActivityCommandHandler> logger)
        {
            this.activityRepository = activityRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<bool> Handle(RequestJoinActivityCommand request, CancellationToken cancellationToken)
        {
            Activity? activity = await activityRepository.GetByIdAsync(request.ActivityId,cancellationToken);
            if (activity == null) 
            {
                logger.LogWarning("用户 {UserId} 尝试加入一个不存在的活动 {ActivityId}", request.UserId, request.ActivityId);
                return false;
            }
            try
            {
                activity.RequestJoinActivity(request.UserId, request.Role);
            }
            catch (InvalidOperationException)
            {
                logger.LogWarning("用户 {UserId} 加入活动 {ActivityId} 失败: {ErrorMessage}", request.UserId, request.ActivityId, ex.Message);
                return false;
            }
            try
            {
                int result = await unitOfWork.SaveChangesAsync(cancellationToken);
                return result > 0;
            }
            catch (DbUpdateConcurrencyException ex)
            {

                logger.LogWarning(ex, "并发冲突: 用户 {UserId} 尝试加入活动 {ActivityId} 失败。数据已被他人修改。", request.UserId, request.ActivityId);
                return false;
            }
            catch (Exception ex)
            {
                // 捕获其他可能的数据库保存错误
                logger.LogError(ex, "保存更改时发生未知错误, ActivityId: {ActivityId}, UserId: {UserId}", request.ActivityId, request.UserId);
                return false;
            }
        }
    }
}
