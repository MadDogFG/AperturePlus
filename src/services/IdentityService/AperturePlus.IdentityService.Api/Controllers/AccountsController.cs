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
            RegisterCommand command = new RegisterCommand(registerRequest.Username, registerRequest.Password, registerRequest.Email);
            var validationResult = await registerCommandValidator.ValidateAsync(command);//异步方法只能手动验证，而且自动验证在新版本的FluentValidation已不支持
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }

            IdentityResult result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Created("", new { Message = "用户成功注册" });//返回201更规范一点
            }
            string errorMsg = $"用户注册失败:\n{string.Join(";\n\n", result.Errors.Select(e => $"{e.Code}:{e.Description}"))}";
            return BadRequest(new { Message = errorMsg });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginRequest loginRequest)
        {
            LoginCommand command = new LoginCommand(loginRequest.LoginIdentifier, loginRequest.Password);
            var validationResult = await new LoginCommandValidator().ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }

            LoginResult result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(new { Token= result.Token,Message = "用户成功登录" });//返回201更规范一点
            }
            return Unauthorized(new { Errors = result.Errors });
        }
    }
}
