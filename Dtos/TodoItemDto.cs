    using TodoApi.Models;

    namespace TodoApi.Dtos;

    // DTO usado para entrada e saída da API
    public class TodoItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Data local (não UTC)
        public DateTime Data { get; set; } = DateTime.Now;

        public string Email { get; set; } = string.Empty;

        public bool IsComplete { get; set; }

        public TodoItemDto() { }

        // Converte Model → DTO
        public TodoItemDto(Todo todo)
        {
            Id = todo.Id;
            Name = todo.Name;
            Data = todo.Data;
            Email = todo.Email;
            IsComplete = todo.IsComplete;
        }
    }