using AperturePlus.IdentityService.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Commands
{
    public record class LoginCommand:IRequest<IdentityResult>
    {

        public LoginIdentifier loginIdentifier { get; init; } //登录标识，可以是用户名或邮箱等
        public string password { get; init; } //密码
        public LoginCommand(string loginIdentifierValue, string password)
        {
            this.loginIdentifier = LoginIdentifier.Create(loginIdentifierValue);
            this.password = password;
        }
    }
}
