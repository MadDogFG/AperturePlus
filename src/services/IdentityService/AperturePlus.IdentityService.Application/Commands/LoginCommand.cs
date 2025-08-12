using AperturePlus.IdentityService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Commands
{
    public record class LoginCommand
    {

        public LoginIdentifier loginIdentifier { get; init; } //登录标识，可以是用户名或邮箱等
        public string Password { get; init; } //密码
        public LoginCommand(string loginIdentifierValue, string password)
        {
            this.loginIdentifier = LoginIdentifier.Create(loginIdentifierValue);
            this.Password = password;
        }
    }

    
}
