namespace backend.Models
{
    public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;

    // Relation 1:N with Note
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}

}
