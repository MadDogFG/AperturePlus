using AperturePlus.UserProfileService.Application.Commands;
using AperturePlus.UserProfileService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AperturePlus.UserProfileService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserProfileController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("GetUserProfileById/{id}")]
        public async Task<IActionResult> GetUserProfileById(Guid id)
        {
            var result = await mediator.Send(new GetUserProfileByIdQuery(id));
            if (result==null)
            {
                return NotFound(new { Message = "未能找到用户档案" });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetMyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString)|| !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var result = await mediator.Send(new GetUserProfileByIdQuery(userId));
            if (result == null)
            {
                return NotFound(new { Message = "您的用户档案尚未创建，请稍后再试。" });
            }
            return Ok(result);
        }
    }
}
