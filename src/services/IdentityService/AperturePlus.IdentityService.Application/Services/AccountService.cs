using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.Interfaces;
using AperturePlus.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AperturePlus.IdentityService.Domain.ValueObjects;
using static AperturePlus.IdentityService.Domain.ValueObjects.LoginIdentifier;
using Microsoft.EntityFrameworkCore;

namespace AperturePlus.IdentityService.Application.Services
{
    public class AccountService:IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AccountService> logger;

        public AccountService(UserManager<ApplicationUser> userManager, ILogger<AccountService> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IdentityResult> RegisterAccountAsync (RegisterCommand registerCommand)
        {
            ApplicationUser user = ApplicationUser.CreateWithEmail(registerCommand.username, registerCommand.email);
            IdentityResult result = await userManager.CreateAsync(user, registerCommand.password);
            return result;
        }

        public async Task<IdentityResult> LoginAccountAsync(LoginCommand loginCommand)
        {
            ApplicationUser user = null;
            if (loginCommand.loginIdentifier.Type == LoginIdentifierType.Email)
            {
                user = await userManager.FindByEmailAsync(loginCommand.loginIdentifier.Value);
            }
            else if (loginCommand.loginIdentifier.Type == LoginIdentifierType.Username)
            {
                user = await userManager.FindByNameAsync(loginCommand.loginIdentifier.Value);
            }
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, loginCommand.Password))
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
