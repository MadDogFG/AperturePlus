using AperturePlus.ActivityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.Interfaces
{
    public interface IUserSummaryRepository
    {
        Task AddUserSummaryAsync(UserSummary userSummary, CancellationToken cancellationToken);
        void UpdateUserSummary(UserSummary userSummary);
        void DeleteUserSummaryAsync(UserSummary userSummary);
        Task<UserSummary?> GetByIdAsync(Guid userSummaryId, CancellationToken cancellationToken);
        Task<List<UserSummary>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<UserSummary>> GetByIdsAsync(IEnumerable<Guid> userSummaryIds, CancellationToken cancellationToken);

    }
}
