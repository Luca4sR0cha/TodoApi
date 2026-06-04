namespace TodoApi.Models;

public class Todo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    public string? Telefone { get; set; }
    public string? Email { get; set; }
}