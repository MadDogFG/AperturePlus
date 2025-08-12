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
            this.userManager = userManager;
            RuleFor(x => x.username).NotEmpty().WithMessage("用户名不能为空。").MustAsync(async (username, cancellationToken) =>
            {
                Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                if (await userManager.FindByNameAsync(username) != null)//判断用户名是否已存在
                {
                    return false;
                }
                return true;
            }).WithMessage("用户名已存在");//待补，用户名不能为邮箱或手机号格式

            RuleFor(c => c).Must( command =>
            {
                if (string.IsNullOrEmpty(command.email))//未来在这里判断是否有一个注册要求的手机号或者邮箱，必须得有一个
                {
                    return false;
                }
                return true;
            });

            RuleFor(x => x.email).MustAsync(async (email, cancellationToken) =>
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
