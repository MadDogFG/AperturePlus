using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Commands
{
    public class UpdateUserRolesCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }

        public UpdateUserRolesCommand(Guid userId, List<string> roles)
        {
            UserId = userId;
            Roles = roles ?? new List<string>();
        }
    }
}