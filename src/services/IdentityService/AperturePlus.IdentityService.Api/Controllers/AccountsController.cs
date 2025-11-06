using AperturePlus.IdentityService.Api.DTOs;
using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.DTOs;
using AperturePlus.IdentityService.Application.Validators;
using AperturePlus.IdentityService.Domain.Entities;
using AperturePlus.IdentityService.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AperturePlus.IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> logger;
        private readonly IValidator<RegisterCommand> registerCommandValidator;
        private readonly IMediator mediator;
        public AccountsController(ILogger<AccountsController> logger,IValidator<RegisterCommand> registerCommandValidator,IMediator mediator)
        {
            this.logger = logger;
            this.registerCommandValidator = registerCommandValidator;
            this.mediator = mediator;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterRequest registerRequest)
        {
            logger.LogInformation("正在为{UserName}注册为新用户, 邮箱: {Email}", registerRequest.Username,registerRequest.Email);
            RegisterCommand command = new RegisterCommand(registerRequest.Username, registerRequest.Password, registerRequest.Email);
            var validationResult = await registerCommandValidator.ValidateAsync(command);//异步方法只能手动验证，而且自动验证在新版本的FluentValidation已不支持
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                logger.LogError("注册失败，验证错误: {Errors}", string.Join(";\n", errors));
                return BadRequest(new { Errors = errors });
            }

            IdentityResult result = await mediator.Send(command);
            if (result.Succeeded)
            {
                logger.LogInformation("用户{UserName}注册成功", registerRequest.Username);
                return Created("", new { Message = "用户成功注册" });//返回201更规范一点
            }
            string errorMsg = $"用户注册失败:\n{string.Join(";\n\n", result.Errors.Select(e => $"{e.Code}:{e.Description}"))}";
            logger.LogError(errorMsg);
            return BadRequest(new { Message = errorMsg });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginRequest loginRequest)
        {
            logger.LogInformation("用户{LoginIdentifier}正在尝试登录", loginRequest.LoginIdentifier);
            LoginCommand command = new LoginCommand(loginRequest.LoginIdentifier, loginRequest.Password);
            var validationResult = await new LoginCommandValidator().ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                logger.LogError("登录失败，验证错误: {Errors}", string.Join(";\n",errors));
                return BadRequest(new { Errors = errors });
            }

            LoginResult result = await mediator.Send(command);
            if (result.Succeeded)
            {
                logger.LogInformation("用户{LoginIdentifier}登录成功", loginRequest.LoginIdentifier);
                return Ok(new { Token= result.Token,Message = "用户成功登录" });//返回201更规范一点
            }
            logger.LogError("用户{LoginIdentifier}登录失败: {Errors}", loginRequest.LoginIdentifier, string.Join(";\n\n", result.Errors));
            return Unauthorized(new { Errors = result.Errors });
        }

        [Authorize]
        [HttpPut("UpdateRoles")]
        public async Task<IActionResult> UpdateRoles(UpdateRolesRequest request) 
        {
            logger.LogInformation("用户{UserId}正在尝试更新角色", User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                logger.LogError("无效的用户ID: {UserIdString}", userIdString);
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var command = new UpdateUserRolesCommand(userId, request.Roles);
            var result = await mediator.Send(command);
            if (result.Succeeded) 
            {
                logger.LogInformation("用户{UserId}角色更新成功", userId);
                return Ok(new { Message = "用户角色更新成功" });
            }
            string error = string.Join(";\n\n", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            logger.LogError("用户{UserId}角色更新失败: {Errors}", userId, error);
            return BadRequest(new { Message = $"用户角色更新失败:\n{error}"});
        }
    }
}
