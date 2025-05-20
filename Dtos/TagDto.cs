using System.ComponentModel.DataAnnotations;
namespace backend.Dtos
{
    public class TagDto
    {
        public int? Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";
    }
}
