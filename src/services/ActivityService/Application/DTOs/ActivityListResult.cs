using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.DTOs
{
    public record class ActivityListResult
    {
        public IEnumerable<ActivityDto> Items { get; init; }
        public bool HasMore { get; init; }

        public ActivityListResult(IEnumerable<ActivityDto> items, bool hasMore)
        {
            Items = items;
            HasMore = hasMore;
        }
    }
}
