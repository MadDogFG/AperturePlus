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
            ApplicationUser user = ApplicationUser.CreateWithEmail(registerCommand.Username, registerCommand.Email);
            IdentityResult result = await userManager.CreateAsync(user, registerCommand.Password);
            return result;
        }
    }
}
