using AperturePlus.RatingService.Api.DTOs;
using AperturePlus.RatingService.Application.Commands;
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Application.Queries;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Domain.ValueObjects;
using AperturePlus.RatingService.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AperturePlus.RatingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly RatingServiceDbContext ratingServiceDbContext;
        private readonly IRatingRepository ratingRepository;
        private readonly IUnitOfWork unitOfWork;
        public testController(IMediator mediator, RatingServiceDbContext ratingServiceDbContext, IRatingRepository ratingRepository, IUnitOfWork unitOfWork)
        {
            this.mediator = mediator;
            this.ratingServiceDbContext = ratingServiceDbContext;
            this.ratingRepository = ratingRepository;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("seed-test-data")]
        public async Task<IActionResult> SeedTestData()
        {
            // 模拟一个活动完成事件
            var activityId = Guid.NewGuid();
            var userA = Guid.NewGuid(); // 模拟用户A
            var userB = Guid.NewGuid(); // 模拟用户B

            // 1. 模拟创建摘要信息
            var userSummaryA = UserSummary.Create(userA, "测试用户A");
            var userSummaryB = UserSummary.Create(userB, "测试用户B");
            var activitySummary = ActivitySummary.Create(activityId, "一次伟大的测试活动");

            // (这里你需要一种方式把这些摘要信息存入数据库，可以通过仓储或直接用DbContext)
            // 假设你有 _context 注入
            ratingServiceDbContext.UserSummaries.AddRange(userSummaryA, userSummaryB);
            ratingServiceDbContext.ActivitySummaries.Add(activitySummary);

            // 2. 模拟创建待评价记录
            var pendingRating1 = Rating.CreatePending(activityId, userA, userB, RoleType.Model);
            var pendingRating2 = Rating.CreatePending(activityId, userB, userA, RoleType.Photographer);

            await ratingRepository.AddRangeAsync(new[] { pendingRating1, pendingRating2 }, CancellationToken.None);
            await unitOfWork.SaveChangesAsync(CancellationToken.None);

            return Ok(new
            {
                Message = "测试数据已生成!",
                PendingRatingFromAtoB_Id = pendingRating1.RatingId,
                PendingRatingFromBtoA_Id = pendingRating2.RatingId,
                UserA_Id = userA
            });
        }

        [HttpGet("my-received-ratings/{userId}")]
        public async Task<IActionResult> GetMyReceivedRatings(Guid userId)
        {
            var query = new GetReceivedRatingsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("sent/{userId}")]
        public async Task<IActionResult> GetSentRatings(Guid userId)
        {
            var query = new GetSentRatingsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("statistics/{userId}")]
        public async Task<IActionResult> GetStatistics(Guid userId)
        {
            var query = new GetRatingStatsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingRatings(Guid userId)
        {
            var query = new GetPendingRatingsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
