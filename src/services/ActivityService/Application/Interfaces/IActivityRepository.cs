using AperturePlus.ActivityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Interfaces
{
    public interface IActivityRepository
    {
        Task AddActivityAsync(Activity activity,CancellationToken cancellationToken);
        void UpdateActivity(Activity activity);
        void DeleteActivity(Activity activity);
        Task<Activity> GetByIdAsync(Guid activityId, CancellationToken cancellationToken);
        Task<List<Activity>> GetAllAsync(CancellationToken cancellationToken);
    }
}
