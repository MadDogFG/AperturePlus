using AperturePlus.IdentityService.Api.DTOs;
using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.Interfaces;
using AperturePlus.IdentityService.Application.Services;
using AperturePlus.IdentityService.Application.Validators;
using AperturePlus.IdentityService.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AperturePlus.IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> logger;
        private readonly IAccountService accountService;
        private readonly IValidator<RegisterCommand> registerCommandValidator;
        public AccountsController(ILogger<AccountsController> logger, IAccountService accountService,IValidator<RegisterCommand> registerCommandValidator)
        {
            this.logger = logger;
            this.accountService = accountService;
            this.registerCommandValidator = registerCommandValidator;
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

            IdentityResult result = await accountService.RegisterAccountAsync(command);
            if (result.Succeeded)
            {
                return Created("", new { Message = "用户成功注册" });//返回201更规范一点
            }
            string errorMsg = $"用户注册失败:\n{string.Join(";\n\n", result.Errors.Select(e => $"{e.Code}:{e.Description}"))}";
            return BadRequest(new { Message = errorMsg });
        }
    }
}
