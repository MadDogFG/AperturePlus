using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IdentityResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Email))
            {
                if(await userManager.FindByNameAsync(request.Username) != null)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "RegisterCommandHandler", Description = "用户名已存在" });
                }
                if(await userManager.FindByEmailAsync(request.Email) != null)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "RegisterCommandHandler", Description = "邮箱已存在" });
                }
                ApplicationUser user = ApplicationUser.CreateWithEmail(request.Username, request.Email);
                IdentityResult result = await userManager.CreateAsync(user, request.Password);
                return result;
            }
            //未来这里可以扩展为手机号注册
            return IdentityResult.Failed(new IdentityError { Code = "RegisterCommandHandler", Description = "注册失败，登录标识不能为空" });
        }
    }
}
