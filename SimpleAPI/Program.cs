using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using SimpleAPI;
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FilmContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost");
                      });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/films", [EnableCors(MyAllowSpecificOrigins)] async (FilmContext db) => await db.Films.ToListAsync());

app.MapGet("/api/films/{FilmID}", [EnableCors(MyAllowSpecificOrigins)] async (FilmContext db, int FilmID) => await db.Films.FindAsync(FilmID));

app.MapPost("/api/films", [EnableCors(MyAllowSpecificOrigins)] async (FilmContext db, Film film) =>
{
    await db.Films.AddAsync(film);
    await db.SaveChangesAsync();
    Results.Accepted();
});

app.MapPut("/api/films/{FilmID}", [EnableCors(MyAllowSpecificOrigins)] async (FilmContext db, int FilmID, Film film) =>
{
    if (FilmID != film.FilmID) return Results.BadRequest();

    db.Update(film);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/api/films/{FilmID}", [EnableCors(MyAllowSpecificOrigins)] async (FilmContext db, int FilmID) =>
{
    var film = await db.Films.FindAsync(FilmID);
    if (film == null) return Results.NotFound();

    db.Films.Remove(film);
    await db.SaveChangesAsync();

    return Results.NoContent();
});



app.Run();

