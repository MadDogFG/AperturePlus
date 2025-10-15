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
    public class RatingsController : ControllerBase
    {
        private readonly IMediator mediator;
        public RatingsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpPost("submit/{ratingId}")]
        public async Task<IActionResult> SubmitRating(Guid ratingId, [FromBody] SubmitRatingRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }

            var command = new SubmitRatingCommand(
                ratingId,
                userId,
                request.Score,
                request.Comments
            );
            var result = await mediator.Send(command);
            if(result==true)
            {
                return Ok(new { Message = "评价成功" });
            }
            else
            {
                return BadRequest(new { Message = "评价失败" });
            }
        }

        [Authorize]
        [HttpGet("my-received-ratings")]
        public async Task<IActionResult> GetMyReceivedRatings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var query = new GetReceivedRatingsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("sent")]
        public async Task<IActionResult> GetSentRatings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var query = new GetSentRatingsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var query = new GetRatingStatsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRatings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var query = new GetPendingRatingsQuery(userId);
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
