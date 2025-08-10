using AperturePlus.IdentityService.Api.DTOs;
using AperturePlus.IdentityService.Application.Services;
using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AperturePlus.IdentityService.Application.Interfaces;

namespace AperturePlus.IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> logger;
        private readonly IAccountService accountService;
        public AccountsController(ILogger<AccountsController> logger, IAccountService accountService)
        {
            this.logger = logger;
            this.accountService = accountService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterRequest registerRequest)
        {
            IdentityResult result = await accountService.RegisterAccountAsync(new RegisterCommand(registerRequest.Username, registerRequest.Password, registerRequest.Email));
            if (result.Succeeded)
            {
                return Created("", new { Message = "用户成功注册" });//返回201更规范一点
            }
            string errorMsg = $"用户注册失败:\n{string.Join(";\n\n", result.Errors.Select(e => $"{e.Code}:{e.Description}"))}";
            return BadRequest(new { Message = errorMsg });
        }
    }
}
