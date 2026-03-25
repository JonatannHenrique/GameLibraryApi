using GameLibraryApi.Data;
using GameLibraryApi.Dtos;
using GameLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryApi.Endpoints;

public static class GamesListEndpoints
{
    public static void MapGamesListEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games");

        // GET /games - Lista todos
        group.MapGet("/", async (AppDbContext db) =>
        {
            var games = await db.Games
                .Include(g => g.Genres)
                .ToListAsync();

            var dtos = games.Select(g => new GameDto
            {
                Id = g.Id,
                Title = g.Title,
                Price = g.Price,
                ReleaseDate = g.ReleaseDate,
                Genres = g.Genres.Select(gen => gen.Name).ToList()
            }).ToList();

            return Results.Ok(dtos);
        })
        .WithName("GetAllGames");

        // GET /games/{id} - Por ID
        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var game = await db.Games
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return Results.NotFound();

            var dto = new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                Genres = game.Genres.Select(gen => gen.Name).ToList()
            };

            return Results.Ok(dto);
        })
        .WithName("GetGameById");
    }
}