using AperturePlus.IdentityService.Domain.Entities;
using AperturePlus.IdentityService.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Infrastructure.Persistence
{
    public class DataSeeder
    {
        private readonly IOptionsSnapshot<RoleSettings> optionsSnapshot;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IConfiguration configuration;

        public DataSeeder(IOptionsSnapshot<RoleSettings> optionsSnapshot, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            this.optionsSnapshot = optionsSnapshot;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        public async Task<IdentityResult> SeedAsync()
        {
            IdentityResult result = IdentityResult.Success;
            result = await SeedRolesAsync();
            if (result.Succeeded)
            {
                result = await SeedAdminAsync();
            }
            return result;
        }

        public async Task<IdentityResult> SeedRolesAsync()
        {
            IdentityResult result = IdentityResult.Success;
            foreach (var role in optionsSnapshot.Value.Roles)
            {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                    result = await roleManager.CreateAsync(ApplicationRole.Create(role));
                }
                if(!result.Succeeded)
                {
                    return result;
                }
            }
            return result;
        }

        public async Task<IdentityResult> SeedAdminAsync()
        {
            IdentityResult result = IdentityResult.Success;
            if (await userManager.FindByNameAsync(configuration.GetSection("AdminUser")["UserName"]) == null)
            {
                ApplicationUser user = ApplicationUser.CreateWithEmail(configuration.GetSection("AdminUser")["UserName"], configuration.GetSection("AdminUser")["Email"]);
                result = await userManager.CreateAsync(user, configuration.GetSection("AdminUser")["Password"]);
                if (result.Succeeded) 
                { 
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            return result;
        }
    }
}
