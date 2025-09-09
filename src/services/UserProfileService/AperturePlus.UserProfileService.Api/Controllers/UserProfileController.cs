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

        [Authorize]
        [HttpGet("GetUserProfileById/{id}")]
        public async Task<IActionResult> GetOrCreateMyProfile(Guid id)
        {
            var result = await mediator.Send(new GetUserProfileByIdQuery(id));
            if (result==null)
            {
                return NotFound(new { Message = "未能找到用户档案" });
            }
            return Ok(result);
        }
    }
}
