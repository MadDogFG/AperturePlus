using AperturePlus.ActivityService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Queries
{
    public class GetAllActivityQuery:IRequest<IEnumerable<ActivityDto>>
    {
    }
}
