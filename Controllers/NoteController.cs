using backend.Models;
using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{

// Agregá esto encima de la clase:
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly NoteService _service;
        private readonly TagRepository _tagRepository;

        public NoteController(NoteService service, TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
            _service = service;
        }
       
        // From JWT
        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);


        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
        {
            int userId = GetUserId();
            var notes = await _service.GetAllAsync(userId);
            if (notes == null) return NotFound("No notes found.");

            var noteDto = notes.Select(note => new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreationDate = note.CreationDate,
                UserId = note.UserId,
                Tags = note.NoteTags.Select(nt => nt.Tag.Name).ToList()
            });
            return Ok(noteDto);
         }

        [HttpGet("archived")]
        public async Task<ActionResult<IEnumerable<Note>>> GetArchived()
        {
            int userId = GetUserId();
            var notes = await _service.GetArchivedAsync(userId);
            if (notes == null) return NotFound("No archived notes found.");

            var noteDto = notes.Select(note => new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreationDate = note.CreationDate,
                UserId = note.UserId,
                Tags = note.NoteTags.Select(nt => nt.Tag.Name).ToList()
            }).ToList();
            return Ok(noteDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> Get(int id)
        {
            int userId = GetUserId();
            var note = await _service.GetNoteByIdAsync(id);
            if (note == null || note.UserId != userId) return NotFound();
            var noteDto = new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                IsArchived = note.IsArchived,
                CreationDate = note.CreationDate,
                UserId = note.UserId,
            };
            noteDto.Tags = note.NoteTags.Select(nt => nt.Tag.Name).ToList();
            return Ok(noteDto);
        }

        [HttpPost]
        public async Task<ActionResult> Create(NoteDto noteDto)
        {
            int userId = GetUserId();
            var note = new Note
            {
                Title = noteDto.Title,
                Content = noteDto.Content,
                IsArchived = noteDto.IsArchived,
                CreationDate = DateTime.UtcNow,
                UserId = userId
            };
            await _service.AddAsync(note, noteDto.Tags);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, noteDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, NoteDto noteDto)
        {
            int userId = GetUserId();
            var note = await _service.GetNoteByIdAsync(id);
            if (note == null || note.UserId != userId) return NotFound();
            if (noteDto.Title == null)
                return BadRequest("Title cannot be null.");

            note.Content = noteDto.Content;
            note.Title = noteDto.Title;
            note.IsArchived = noteDto.IsArchived;
            note.UserId = userId;
            
            await _service.UpdateAsync(note, noteDto.Tags);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            int userId = GetUserId();
            var note = await _service.GetNoteByIdAsync(id);
            if (note == null || note.UserId != userId) return NotFound();
            await _service.DeleteAsync(note);
            return NoContent();
        }

        [HttpPatch("{id}/archive")]
        public async Task<ActionResult> Archive(int id)
        {
            int userId = GetUserId();
            var note = await _service.GetNoteByIdAsync(id);
            if (note == null || note.UserId != userId) return NotFound();
            note.IsArchived = !note.IsArchived;
            await _service.UpdateAsync(note);
            return NoContent();
        }
    }
}
