using AperturePlus.RatingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Interfaces
{
    public interface IUserSummaryRepository
    {
        public Task AddAsync(ActivitySummary activitySummary, CancellationToken cancellationToken);

        public Task<ActivitySummary?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    }
}
