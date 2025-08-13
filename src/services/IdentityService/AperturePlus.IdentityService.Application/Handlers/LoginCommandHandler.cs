using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.DTOs;
using AperturePlus.IdentityService.Application.Interfaces;
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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager,IJwtTokenGenerator jwtTokenGenerator)
        {
            this.userManager = userManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = null;
            if (request.Identifier.Type == LoginIdentifierType.Email)
            {
                user = await userManager.FindByEmailAsync(request.Identifier.Value);
            }
            else if (request.Identifier.Type == LoginIdentifierType.Username)
            {
                user = await userManager.FindByNameAsync(request.Identifier.Value);
            }
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, request.Password))
                {
                    string token = jwtTokenGenerator.GenerateToken(user);
                    if(string.IsNullOrEmpty(token))
                    {
                        return LoginResult.Failure(new[] {"无法生成Token"});
                    }
                    return LoginResult.Success(token); //登录成功
                }
                else
                {
                    return LoginResult.Failure(new [] {"登录失败，密码错误" });
                }
            }
            else
            {
                return LoginResult.Failure(new [] {"登录失败，用户不存在" });
            }
        }
    }
}
