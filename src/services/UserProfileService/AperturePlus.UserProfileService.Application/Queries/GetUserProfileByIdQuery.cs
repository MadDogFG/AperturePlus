using AperturePlus.UserProfileService.Application.DTOs;
using AperturePlus.UserProfileService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.Queries
{
    public class GetUserProfileByIdQuery:IRequest<UserProfileDto?>
    {
        public Guid UserId { get; private set; }

        public GetUserProfileByIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
