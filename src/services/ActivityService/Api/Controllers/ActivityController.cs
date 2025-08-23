using AperturePlus.ActivityService.Api.DTOs;
using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AperturePlus.ActivityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IMediator mediator;
        public ActivityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpPost("CreateActivity")]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequest request)
        {
            CreateActivityCommand command = new CreateActivityCommand(
                request.ActivityTitle,
                request.ActivityDescription,
                request.ActivityLocation,
                request.ActivityStartTime,
                Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))//从User.Claims中获取用户ID（ControllerBase提供的）
            );
            var result = await mediator.Send(command);
            if (result.Successed)
            {
                return Created("", new { Message = "活动创建成功", ActivityId = result.ActivityId });
            }
            else
            {
                return BadRequest(new { Message = result.Error });
            }

        }

        [HttpGet("GetAllActivity")]
        public async Task<IActionResult> GetAllActivity()
        {
            var result = await mediator.Send(new GetAllActivityQuery());
            return Ok(result);
        }

        [HttpGet("GetActivityById/{id}")]
        public async Task<IActionResult> GetActivityById(Guid id)
        {
            var result = await mediator.Send(new GetActivityByIdQuery(id));
            if(result == null)
            {
                return NotFound(new { Message = "活动未找到" });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPut("UpdateActivity/{id}")]
        public async Task<IActionResult> UpdateActivity(Guid id,UpdateActivityRequest updateActivityRequest)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            UpdateActivityCommand command = new UpdateActivityCommand(
                id,
                updateActivityRequest.ActivityTitle,
                updateActivityRequest.ActivityDescription,
                updateActivityRequest.ActivityLocation,
                updateActivityRequest.ActivityStartTime,
                userId
            );
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest(new { Message = "更新失败" });
            }
            return Ok(new { Message = "更新成功" });
        }

        [Authorize]
        [HttpPost("CancelActivity/{activityId}")]
        public async Task<IActionResult> CancelActivity(Guid activityId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            CancelActivityCommand command = new CancelActivityCommand(activityId, userId);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest(new { Message = "取消失败" });
            }
            return Ok(new { Message = "取消成功" });
        }

        [Authorize]
        [HttpPost("CompletedActivity/{activityId}")]
        public async Task<IActionResult> CompletedActivity(Guid activityId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            CompletedActivityCommand command = new CompletedActivityCommand(activityId, userId);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest(new { Message = "完成活动失败" });
            }
            return Ok(new { Message = "完成活动成功" });
        }
    }
}
