using ScrumPilot.Shared.Models;

namespace ScrumPilot.Data.Repositories
{
    public interface IEpicRepository
    {
        Task<IEnumerable<Epic>> GetAllAsync();
        Task<Epic?> GetByIdAsync(int id);
        Task<Epic> AddAsync(Epic epic);
        Task<Epic> UpdateAsync(Epic epic);
        Task<bool> DeleteAsync(int id);
    }
}
