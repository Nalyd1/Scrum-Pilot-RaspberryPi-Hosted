using Microsoft.EntityFrameworkCore;
using ScrumPilot.Data.Context;
using ScrumPilot.Shared.Models;

namespace ScrumPilot.Data.Repositories
{
    public class EpicRepository : IEpicRepository
    {
        private readonly ScrumPilotContext _context;

        public EpicRepository(ScrumPilotContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Epic>> GetAllAsync()
        {
            return await _context.Epics
                .OrderByDescending(e => e.DateCreated)
                .ToListAsync();
        }

        public async Task<Epic?> GetByIdAsync(int id)
        {
            return await _context.Epics.FindAsync(id);
        }

        public async Task<Epic> AddAsync(Epic epic)
        {
            epic.DateCreated = DateTime.UtcNow;
            _context.Epics.Add(epic);
            await _context.SaveChangesAsync();
            return epic;
        }

        public async Task<Epic> UpdateAsync(Epic epic)
        {
            _context.Entry(epic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return epic;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var epic = await _context.Epics.FindAsync(id);
            if (epic == null) return false;

            _context.Epics.Remove(epic);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
