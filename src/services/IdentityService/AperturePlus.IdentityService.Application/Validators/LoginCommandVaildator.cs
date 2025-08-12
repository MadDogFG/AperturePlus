using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Domain.Entities;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Validators
{
    public class LoginCommandVaildator : AbstractValidator<LoginCommand>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public LoginCommandVaildator(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            RuleFor(l => l.loginIdentifier).NotEmpty().WithMessage("登录标识不能为空");
        }
    }
}
