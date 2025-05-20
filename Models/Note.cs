namespace backend.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreationDate { get; set; }

        public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
