using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class NoteService
    {
        private readonly NoteRepository _noteRepository;
        private readonly TagRepository _tagRepository;
        private readonly NoteTagRepository _noteTagRepository;

        public NoteService(NoteRepository noteRepository, TagRepository tagRepository, NoteTagRepository noteTagRepository)
        {
            _noteRepository = noteRepository;
            _tagRepository = tagRepository;
            _noteTagRepository = noteTagRepository;
        }

        public Task<List<Note>> GetAllAsync(int UserId) => _noteRepository.GetAllAsync(UserId);

        public Task<List<Note>> GetArchivedAsync(int UserId) => _noteRepository.GetArchivedAsync(UserId);
        public Task<Note?> GetNoteByIdAsync(int id) => _noteRepository.GetByIdAsync(id);

        public async Task AddAsync(Note note, List<string>? tagNames = null)
        {
            await _noteRepository.AddAsync(note);

            if (tagNames != null && tagNames.Any())
            {
                foreach (var tagName in tagNames)
                {
                    var existingTag = await _tagRepository.GetByNameAsync(tagName);
                    Tag tag;
                    if (existingTag == null)
                    {
                        tag = new Tag { Name = tagName };
                        await _tagRepository.AddAsync(tag);
                    }
                    else
                    {
                        tag = existingTag;
                    }

                    await _noteTagRepository.AddTagToNoteAsync(note.Id, tag.Id);
                }
            }
        }

        public async Task UpdateAsync(Note note, List<string>? tagNames = null)
        {
            await _noteRepository.UpdateAsync(note);

            if (note != null)
            {
                //Clear existing tags for the note
                var existingNoteTags = await _noteTagRepository.GetByNoteIdAsync(note.Id);
                foreach (var noteTag in existingNoteTags)
                {
                    await _noteTagRepository.DeleteAsync(noteTag);
                }

                if(tagNames != null)
                {
                    //Add new tags
                    foreach (var tagName in tagNames)
                    {
                        var existingTag = await _tagRepository.GetByNameAsync(tagName);
                        Tag tag;
                        if (existingTag == null)
                        {
                            tag = new Tag { Name = tagName };
                            await _tagRepository.AddAsync(tag);
                        }
                        else
                        {
                            tag = existingTag;
                        }
                        await _noteTagRepository.AddTagToNoteAsync(note.Id, tag.Id);
                    }
                }   
            }
        }

        public Task DeleteAsync(Note note) => _noteRepository.DeleteAsync(note);

        public async Task AddTagToNoteAsync(int noteId, string tagName)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            var tag = await _tagRepository.GetByNameAsync(tagName);

            if (note != null && tag != null)
            {
                await _noteTagRepository.AddTagToNoteAsync(noteId, tag.Id);
            }
        }

        public async Task RemoveTagFromNoteAsync(int noteId, string tagName)
        {
            var tag = await _tagRepository.GetByNameAsync(tagName);
            if (tag != null)
            {
                await _noteTagRepository.RemoveTagFromNoteAsync(noteId, tag.Id);
            }
        }
    }
}