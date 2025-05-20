using System.ComponentModel.DataAnnotations;
namespace backend.Dtos
{
    public class NoteDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = "";
        [Required]
        public string Content { get; set; } = "";
        public bool IsArchived { get; set; } = false;
        public List<string> Tags { get; set; }
        public DateTime CreationDate;
        // Nuevos campos para la relación con el usuario
        public int UserId { get; set; }
    }
}
