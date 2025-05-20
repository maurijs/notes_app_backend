using backend.Models;
using backend.Dtos;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly TagRepository _tagRepository;

        public TagController(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        // GET: api/tag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            var tags = await _tagRepository.GetAllAsync();
            if (tags == null || !tags.Any())
                return NotFound("No tags found.");
            var tagsDto = tags.Select(tag => new TagDto
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList(); 
            
            return Ok(tagsDto);
        }

        // GET: api/tag/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> Get(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return Ok(tag);
        }

        // POST: api/tag
        [HttpPost]
        public async Task<ActionResult<TagDto>> Create(TagDto tag)
        {
            Tag? existing = await _tagRepository.GetByNameAsync(tag.Name);
            if (existing != null)
                return Conflict("A tag with this name already exists.");

            Tag newTag = new Tag
            {
                Name = tag.Name
            };

            Tag createdTag = await _tagRepository.AddAsync(newTag);

            if (createdTag == null)
            {
                return StatusCode(500, "Failed to retrieve the newly created tag.");
            }

            var responseDto = new TagDto
            {
                Id = createdTag.Id,
                Name = createdTag.Name
            };

            return CreatedAtAction(nameof(Get), new { id = createdTag.Id }, responseDto);
        }

        // PUT: api/tag/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TagDto updatedTag)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return NotFound();

            tag.Name = updatedTag.Name;
            await _tagRepository.UpdateAsync(tag);

            return NoContent();
        }

        // DELETE: api/tag/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return NotFound();

            await _tagRepository.DeleteAsync(tag);
            return NoContent();
        }
    }
}
