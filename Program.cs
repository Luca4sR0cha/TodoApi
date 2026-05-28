using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Models;

// Cria o builder da aplicação ASP.NET.
// Ele é responsável por configurar serviços, dependências e opções da aplicação.
var builder = WebApplication.CreateBuilder(args);

// Registra o contexto do Entity Framework no container de injeção de dependência.
// Aqui estamos dizendo que o TodoDb usará SQL Server e a connection string "DefaultConnection" definida no appsettings.json.
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona um filtro de exceções voltado ao desenvolvimento.
// Ele ajuda a exibir erros relacionados ao banco de dados de forma mais detalhada, para o desenvolvedor.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Constrói a aplicação com base nas configurações definidas acima.
var app = builder.Build();

// Cria um grupo de rotas com o prefixo "/todoitems".
// Isso evita repetir esse trecho em todos os endpoints.
var todoItems = app.MapGroup("/todoitems");


// =========================
// GET /todoitems
// GET /todoitems?isComplete=true
// =========================
// Retorna todos os itens.
// Se o parâmetro opcional "isComplete" for enviado, filtra pelos itens completos ou incompletos.
todoItems.MapGet("/", GetTodoItems);

static async Task<IResult> GetTodoItems(bool? isComplete, TodoDb db)
{
    // Começa a consulta a partir da tabela Todos.
    var query = db.Todos.AsQueryable();

    // Se o usuário informou o parâmetro isComplete, aplica o filtro correspondente.
    if (isComplete.HasValue)
    {
        query = query.Where(t => t.IsComplete == isComplete.Value);
    }

    // Ordena por Id e converte para DTO.
    var items = await query
        .OrderBy(t => t.Id)
        .Select(t => new TodoItemDto(t))
        .ToListAsync();

    // Retorna HTTP 200 OK com a lista de itens.
    return Results.Ok(items);
}


// =========================
// GET /todoitems/{id}
// =========================
// Retorna um item específico pelo Id.
todoItems.MapGet("/{id:int}", async (int id, TodoDb db) =>
{
    // Procura o item pelo Id informado.
    var todo = await db.Todos.FindAsync(id);

    // Se não encontrou, retorna HTTP 404 Not Found.
    if (todo is null)
        return Results.NotFound();

    // Se encontrou, retorna DTO.
    return Results.Ok(new TodoItemDto(todo));
});


// =========================
// POST /todoitems
// =========================
// Cria um novo item no banco.
todoItems.MapPost("/", async (TodoItemDto input, TodoDb db) =>
{
    // Validação simples: o Name é obrigatório.
    if (string.IsNullOrWhiteSpace(input.Name))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = new[] { "The Name field is required." }
        });
    }

    // Cria a entidade Todo a partir do DTO recebido.
    var todo = new Todo
    {
        Name = input.Name.Trim(),
        IsComplete = input.IsComplete,
        Email = input.Email,          // 
        Data = DateTime.Now           // (melhor prática: servidor controla a data)
    };

    // Adiciona o novo item ao contexto.
    db.Todos.Add(todo);

    // Persiste as alterações no banco.
    await db.SaveChangesAsync();

    // Retorna HTTP 201 Created com o recurso criado.
    return Results.Created($"/todoitems/{todo.Id}", new TodoItemDto(todo));
});


// =========================
// PUT /todoitems/{id}
// =========================
// Atualiza completamente um item existente.
todoItems.MapPut("/{id:int}", async (int id, TodoItemDto input, TodoDb db) =>
{
    // Validação simples: o Name é obrigatório.
    if (string.IsNullOrWhiteSpace(input.Name))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = new[] { "The Name field is required." }
        });
    }

    // Busca o item existente.
    var todo = await db.Todos.FindAsync(id);

    // Se não existir, retorna 404.
    if (todo is null)
        return Results.NotFound();

    // Atualiza campos.
    todo.Name = input.Name.Trim();
    todo.IsComplete = input.IsComplete;
    todo.Email = input.Email; // (opcional mas correto manter consistência)

    // Salva alterações.
    await db.SaveChangesAsync();

    return Results.NoContent();
});


// =========================
// PATCH /todoitems/{id}
// =========================
// Atualiza parcialmente um item existente.
todoItems.MapPatch("/{id:int}", async (int id, TodoPatchDto input, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
        return Results.NotFound();

    if (input.Name is not null)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["name"] = new[] { "The Name field cannot be empty." }
            });
        }

        todo.Name = input.Name.Trim();
    }

    if (input.IsComplete.HasValue)
    {
        todo.IsComplete = input.IsComplete.Value;
    }

    await db.SaveChangesAsync();

    return Results.NoContent();
});


// =========================
// DELETE /todoitems/{id}
// =========================
// Remove um item existente.
todoItems.MapDelete("/{id:int}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
        return Results.NotFound();

    db.Todos.Remove(todo);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Inicia a aplicação e começa a escutar requisições HTTP.
app.Run();