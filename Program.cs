using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// 🔥 IMPORTANTE (FRONTEND FUNCIONAR)
app.UseDefaultFiles();   // abre index.html automaticamente
app.UseStaticFiles();    // serve wwwroot
app.UseCors("AllowAll");

//app.Urls.Clear();
//app.Urls.Add("http://localhost:5136");

var todoItems = app.MapGroup("/todoitems");

// GET ALL
todoItems.MapGet("/", async (TodoDb db) =>
{
    return await db.Todos
        .OrderBy(x => x.Id)
        .Select(x => new TodoItemDto(x))
        .ToListAsync();
});

// GET BY ID
todoItems.MapGet("/{id:int}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    return Results.Ok(new TodoItemDto(todo));
});

// POST
todoItems.MapPost("/", async (TodoItemDto input, TodoDb db) =>
{
    var todo = new Todo
    {
        Name = input.Name,
        IsComplete = input.IsComplete,
        Telefone = input.Telefone,
        Email = input.Email,
        Data = DateTime.Now
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", new TodoItemDto(todo));
});

// PUT
todoItems.MapPut("/{id:int}", async (int id, TodoItemDto input, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Name = input.Name;
    todo.IsComplete = input.IsComplete;
    todo.Telefone = input.Telefone;
    todo.Email = input.Email;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// PATCH
todoItems.MapPatch("/{id:int}", async (int id, TodoPatchDto input, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    if (input.Name is not null) todo.Name = input.Name;
    if (input.IsComplete.HasValue) todo.IsComplete = input.IsComplete.Value;
    if (input.Telefone is not null) todo.Telefone = input.Telefone;
    if (input.Email is not null) todo.Email = input.Email;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE
todoItems.MapDelete("/{id:int}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();
