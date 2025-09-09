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
    public class GetOrCreateUserProfileCommand:IRequest<UserProfileDto>
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
