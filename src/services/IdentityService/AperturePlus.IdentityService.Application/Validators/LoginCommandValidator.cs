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
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(l => l.loginIdentifier.value).NotEmpty().WithMessage("登录标识不能为空");
        }
    }
}
