namespace TodoApi.Models;

public class Todo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    // Agora públicos para o EF
    public DateTime Data { get; set; } = DateTime.Now;
    public string Email { get; set; } = string.Empty;
}