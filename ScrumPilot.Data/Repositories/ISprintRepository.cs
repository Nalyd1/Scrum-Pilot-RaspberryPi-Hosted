using ScrumPilot.Shared.Models;

namespace ScrumPilot.Data.Repositories
{
    public interface ISprintRepository
    {
        Task<IEnumerable<Sprint>> GetAllAsync();
        Task<Sprint?> GetByIdAsync(int id);
        Task<Sprint> AddAsync(Sprint sprint);
        Task<Sprint> UpdateAsync(Sprint sprint);
        Task<bool> DeleteAsync(int id);
    }
}
