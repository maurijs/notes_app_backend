using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class NoteTagRepository
    {
        private readonly AppDbContext _context;

        public NoteTagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NoteTag>> GetAllAsync() =>
            await _context.NoteTags.ToListAsync();

        public async Task<NoteTag?> GetByIdsAsync(int noteId, int tagId) =>
            await _context.NoteTags.FindAsync(noteId, tagId);

        public async Task<List<NoteTag>> GetByNoteIdAsync(int noteId) =>
            await _context.NoteTags
                .Where(nt => nt.NoteId == noteId)
                .Include(nt => nt.Tag)
                .ToListAsync();

        public async Task<List<NoteTag>> GetByTagIdAsync(int tagId) =>
            await _context.NoteTags
                .Where(nt => nt.TagId == tagId)
                .Include(nt => nt.Note) 
                .ToListAsync();

        public async Task AddAsync(NoteTag noteTag)
        {
            _context.NoteTags.Add(noteTag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NoteTag noteTag)
        {
            _context.NoteTags.Remove(noteTag);
            await _context.SaveChangesAsync();
        }

        public async Task AddTagToNoteAsync(int noteId, int tagId)
        {
            var exists = await _context.NoteTags.AnyAsync(nt => nt.NoteId == noteId && nt.TagId == tagId);
            if (!exists)
            {
                var noteTag = new NoteTag { NoteId = noteId, TagId = tagId };
                _context.NoteTags.Add(noteTag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveTagFromNoteAsync(int noteId, int tagId)
        {
            var noteTagToRemove = await _context.NoteTags
                .FirstOrDefaultAsync(nt => nt.NoteId == noteId && nt.TagId == tagId);

            if (noteTagToRemove != null)
            {
                _context.NoteTags.Remove(noteTagToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }
}