using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AperturePlus.ActivityService.Application.DTOs;
using MediatR;

namespace AperturePlus.ActivityService.Application.Queries
{
    public record class GetActivityByIdQuery:IRequest<ActivityDto?>
    {
        public Guid ActivityId { get; init; }
        public GetActivityByIdQuery(Guid activityId)
        {
            ActivityId = activityId;
        }
    }
}
