namespace TodoApi.Dtos;

public class TodoPatchDto
{
    public string? Name { get; set; }
    public bool? IsComplete { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
}