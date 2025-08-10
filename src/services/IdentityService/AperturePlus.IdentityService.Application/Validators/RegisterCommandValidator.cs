using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Validators
{
    public class RegisterCommandValidator:AbstractValidator<RegisterCommand>//分层验证，双重保险
    {
        private readonly UserManager<ApplicationUser> userManager;
        public RegisterCommandValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("用户名不能为空。").MustAsync(async (username, cancellationToken) =>
            {
                Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                if (await userManager.FindByNameAsync(username) != null)//判断用户名是否已存在
                {
                    return false;
                }
                return true;
            }).WithMessage("用户名已存在");

            RuleFor(x => x.Email).MustAsync(async (email, cancellationToken) =>
            {
                if (await userManager.FindByEmailAsync(email) != null)//判断邮箱是否已存在
                {
                    return false;
                }
                return true;
            }).WithMessage("邮箱已注册");
        }
      
    }
}
