using AperturePlus.ActivityService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Queries
{
    public class GetByUserIdQuery: IRequest<List<ActivityDto>>
    {
        public Guid UserId { get; init; }
        public GetByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
