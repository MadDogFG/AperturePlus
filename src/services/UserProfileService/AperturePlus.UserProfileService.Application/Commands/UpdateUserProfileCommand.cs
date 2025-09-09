using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Commands
{
    public class UpdateUserProfileCommand:IRequest<bool>
    {
        public Guid UserId { get; private set; }
        public string? Bio {  get; set; }
        public string? AvatarUrl { get; set; }

        public UpdateUserProfileCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
