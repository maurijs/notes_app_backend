using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class NoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Note>> GetAllAsync(int userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId && !n.IsArchived)
                .Include(n => n.NoteTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreationDate)
                .ToListAsync();
        }


        public async Task<List<Note>> GetArchivedAsync(int userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId && n.IsArchived)
                .Include(n => n.NoteTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreationDate)
                .ToListAsync();
        }


        public async Task<Note?> GetByIdAsync(int id) =>
            await _context.Notes
                .Where(n => n.Id == id)
                .Include(n => n.NoteTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync();

        public async Task AddAsync(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Note note)
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Note note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}