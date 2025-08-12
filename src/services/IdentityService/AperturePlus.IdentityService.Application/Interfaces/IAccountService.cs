using AperturePlus.IdentityService.Application.Commands;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AperturePlus.IdentityService.Application.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAccountAsync (RegisterCommand registerRequest);
        Task<IdentityResult> LoginAccountAsync(LoginCommand loginCommand);
    }
}
