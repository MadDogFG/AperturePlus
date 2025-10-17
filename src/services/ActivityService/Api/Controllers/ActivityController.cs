using AperturePlus.ActivityService.Api.DTOs;
using AperturePlus.ActivityService.Application.Commands;
using AperturePlus.ActivityService.Application.Queries;
using AperturePlus.ActivityService.Domain.ValueObjects;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
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
            if (!Enum.TryParse<RoleType>(request.CreatorRole, true, out var creatorRole))
            {
                return BadRequest(new { Message = "无效的创建者角色" });
            }
            CreateActivityCommand command = new CreateActivityCommand(
                request.ActivityTitle,
                request.ActivityDescription,
                request.ActivityLocation,
                request.ActivityStartTime,
                Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),//从User.Claims中获取用户ID（ControllerBase提供的）
                request.Fee,
                request.RoleRequirements,
                creatorRole
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
        public async Task<IActionResult> GetAllActivity(int page,int pagesize)
        {
            var result = await mediator.Send(new GetAllActivityQuery(page,pagesize));
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

        [HttpGet("GetActivitiesByUserId/{id}")]
        public async Task<IActionResult> GetActivitiesByUserId(Guid id)
        {
            var result = await mediator.Send(new GetByUserIdQuery(id));
            if (result == null)
            {
                return NotFound(new { Message = "活动未找到" });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPatch("UpdateActivity/{id}")]
        public async Task<IActionResult> UpdateActivity(Guid id,UpdateActivityRequest updateActivityRequest)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var command = new UpdateActivityCommand(id,userId)
            {
                ActivityTitle = updateActivityRequest.ActivityTitle,
                ActivityDescription = updateActivityRequest.ActivityDescription,
                ActivityLocation = updateActivityRequest.ActivityLocation,
                ActivityStartTime = updateActivityRequest.ActivityStartTime,
                Fee = updateActivityRequest.Fee,
                RoleRequirements = updateActivityRequest.RoleRequirements
            };
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

        [Authorize]
        [HttpPost("RequestJoinActivity/{activityId}")]
        public async Task<IActionResult> RequestJoinActivity(Guid activityId, string roleString)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            if (!Enum.TryParse<RoleType>(roleString, out RoleType roleType)) 
            {
                return BadRequest(new { Message = $"无效角色名:{roleString}" });
            }
            if (!User.IsInRole(roleType.ToString()))
            {
                return Forbid();//返回403，表示无权限
            }
            RequestJoinActivityCommand command = new RequestJoinActivityCommand(activityId, userId,roleType);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest(new { Message = "申请活动失败" });
            }
            return Ok(new { Message = "申请活动成功" });
        }

        [Authorize]
        [HttpPost("ApproveParticipant/{activityId}/{applicantId}")]
        public async Task<IActionResult> ApproveParticipant(Guid activityId, Guid applicantId,string roleString)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            if (!Enum.TryParse<RoleType>(roleString, out RoleType roleType))
            {
                return BadRequest(new { Message = $"无效角色名:{roleString}" });
            }
            ApproveParticipantCommand command = new ApproveParticipantCommand(activityId, userId, applicantId,roleType);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest(new { Message = "批准申请者失败" });
            }
            return Ok(new { Message = "批准申请者成功" });
        }

        [Authorize]
        [HttpPost("RejectParticipant/{activityId}/{applicantId}")]
        public async Task<IActionResult> RejectParticipant(Guid activityId, Guid applicantId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))//防止找不到用户ID或转换失败
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            RejectParticipantCommand command = new RejectParticipantCommand(activityId, userId, applicantId);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest(new { Message = "拒绝申请者失败" });
            }
            return Ok(new { Message = "拒绝申请者成功" });
        }
    }
}
