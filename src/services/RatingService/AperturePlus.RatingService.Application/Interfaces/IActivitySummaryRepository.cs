using AperturePlus.RatingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Interfaces
{
    public interface IActivitySummaryRepository
    {
        public Task AddAsync(ActivitySummary activitySummary, CancellationToken cancellationToken);

        public Task<ActivitySummary?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken);
    }
}
