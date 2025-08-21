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

        [Authorize]
        [HttpGet("GetAllActivity")]
        public async Task<IActionResult> GetAllActivity()
        {
            var result = await mediator.Send(new GetAllActivityQuery());
            return Ok(result);
        }
    }
}
