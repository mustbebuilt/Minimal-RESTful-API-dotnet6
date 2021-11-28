using Microsoft.EntityFrameworkCore;
using SimpleAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FilmContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/films", async (FilmContext db) => await db.Films.ToListAsync());

app.MapGet("/api/films/{FilmID}", async (FilmContext db, int FilmID) => await db.Films.FindAsync(FilmID));

app.MapPost("/api/films", async (FilmContext db, Film film) =>
{
    await db.Films.AddAsync(film);
    await db.SaveChangesAsync();
    Results.Accepted();
});

app.MapPut("/api/films/{FilmID}", async (FilmContext db, int FilmID, Film film) =>
{
    if (FilmID != film.FilmID) return Results.BadRequest();

    db.Update(film);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/api/films/{FilmID}", async (FilmContext db, int FilmID) =>
{
    var film = await db.Films.FindAsync(FilmID);
    if (film == null) return Results.NotFound();

    db.Films.Remove(film);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

