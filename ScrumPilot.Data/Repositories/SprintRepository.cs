using Microsoft.EntityFrameworkCore;
using ScrumPilot.Data.Context;
using ScrumPilot.Shared.Models;

namespace ScrumPilot.Data.Repositories
{
    public class SprintRepository : ISprintRepository
    {
        private readonly ScrumPilotContext _context;

        public SprintRepository(ScrumPilotContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sprint>> GetAllAsync()
        {
            return await _context.Sprints
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<Sprint?> GetByIdAsync(int id)
        {
            return await _context.Sprints.FindAsync(id);
        }

        public async Task<Sprint> AddAsync(Sprint sprint)
        {
            _context.Sprints.Add(sprint);
            await _context.SaveChangesAsync();
            return sprint;
        }

        public async Task<Sprint> UpdateAsync(Sprint sprint)
        {
            _context.Entry(sprint).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return sprint;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint == null) return false;

            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
