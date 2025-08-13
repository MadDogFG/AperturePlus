using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Validators
{
    public class RegisterCommandValidator:AbstractValidator<RegisterCommand>//分层验证，双重保险
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("用户名不能为空");

            RuleFor(c => c).Must( command =>
            {
                if (string.IsNullOrEmpty(command.Email))//未来在这里判断是否有一个注册要求的手机号或者邮箱，必须得有一个
                {
                    MailAddress mailAddress;
                    if(MailAddress.TryCreate(command.Email,out mailAddress))
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage("登录标识不能为空或格式错误");
        }
      
    }
}
