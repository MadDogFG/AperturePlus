using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Commands
{
    public class UpdateUserRolesCommand:IRequest<IdentityResult>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }
        public UpdateUserRolesCommand(Guid userId, List<string> roles)
        {
            UserId = userId;
            Roles = roles;
        }
    }
}
