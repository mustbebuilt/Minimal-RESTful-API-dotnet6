using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using SimpleAPI;
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FilmContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost", "http://localhost:8080").AllowAnyHeader().WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");

                      });
});
var app = builder.Build();


app.UseCors(MyAllowSpecificOrigins);

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

    return Results.Accepted();
});

app.MapDelete("/api/films/{FilmID}", async (FilmContext db, int FilmID) =>
{
    var film = await db.Films.FindAsync(FilmID);
    if (film == null) return Results.NotFound();

    db.Films.Remove(film);
    await db.SaveChangesAsync();

    return Results.Accepted();
});

// Options for Chrome
app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" }, () => "This is an options or head request ");


app.Run();

