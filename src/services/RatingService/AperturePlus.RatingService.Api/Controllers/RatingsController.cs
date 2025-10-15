using AperturePlus.RatingService.Api.DTOs;
using AperturePlus.RatingService.Application.Commands;
using AperturePlus.RatingService.Domain.Entities;
using AperturePlus.RatingService.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
