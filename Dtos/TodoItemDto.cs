using TodoApi.Models;

namespace TodoApi.Dtos;

public class TodoItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool IsComplete { get; set; }

    public TodoItemDto() { }

    public TodoItemDto(Todo todo)
    {
        Id = todo.Id;
        Name = todo.Name;
        Data = todo.Data;
        Telefone = todo.Telefone;
        Email = todo.Email;
        IsComplete = todo.IsComplete;
    }
}