using AperturePlus.UserProfileService.Application.DTOs;
using AperturePlus.UserProfileService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Commands
{
    public class CreateUserProfileCommand:IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }

        public CreateUserProfileCommand(Guid userId, string userName, List<string> roles)
        {
            UserId = userId;
            UserName = userName;
            Roles = roles ?? new List<string>();
        }
    }
}
