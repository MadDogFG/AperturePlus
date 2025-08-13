using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AperturePlus.IdentityService.Domain.ValueObjects.LoginIdentifier;

namespace AperturePlus.IdentityService.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        
        public LoginCommandHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IdentityResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = null;
            if (request.loginIdentifier.type == LoginIdentifierType.Email)
            {
                user = await userManager.FindByEmailAsync(request.loginIdentifier.value);
            }
            else if (request.loginIdentifier.type == LoginIdentifierType.Username)
            {
                user = await userManager.FindByNameAsync(request.loginIdentifier.value);
            }
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, request.password))
                {
                    return IdentityResult.Success; //登录成功
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError { Code = "LoginAccountAsync", Description = "登录失败，密码错误" });
                }
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Code = "LoginAccountAsync", Description = "登录失败，用户不存在" });
            }
        }
    }
}
