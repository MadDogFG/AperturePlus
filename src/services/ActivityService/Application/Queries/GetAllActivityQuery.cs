using AperturePlus.ActivityService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Queries
{
    public class GetAllActivityQuery: IRequest<ActivityListResult>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public GetAllActivityQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
