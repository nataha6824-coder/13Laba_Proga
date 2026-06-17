using _13Laba_Proga;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var str = builder.Configuration.GetConnectionString("SQLite");
if (str == null)
{
    str = "Data Source=library.db";
}

builder.Services.AddDbContext<LibraryContext>(opt => opt.UseSqlite(str));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/books", (LibraryContext db) =>
{
    var list = db.Books.ToList();
    return Results.Ok(list);
});

app.MapGet("/books/{id}", (int id, LibraryContext db) =>
{
    var book = db.Books.Find(id);
    if (book == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(book);
});

app.MapPost("/books", (Book book, LibraryContext db) =>
{
    db.Books.Add(book);
    db.SaveChanges();
    return Results.Created("/books/" + book.Id, book);
});

app.MapPut("/books/{id}", (int id, Book updatedBook, LibraryContext db) =>
{
    var existing = db.Books.Find(id);
    if (existing == null)
    {
        return Results.NotFound();
    }

    existing.Name = updatedBook.Name;
    existing.Author = updatedBook.Author;
    existing.ReleaseDate = updatedBook.ReleaseDate;

    db.SaveChanges();
    return Results.Ok(existing);
});

app.MapDelete("/books/{id}", (int id, LibraryContext db) =>
{
    var x = db.Books.Find(id);
    if (x == null)
    {
        return Results.NotFound();
    }

    db.Books.Remove(x);
    db.SaveChanges();
    return Results.NoContent();
});

app.Run();

public partial class Program { }