using GameLibraryApi.Data;
using GameLibraryApi.Dtos;
using GameLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryApi.Endpoints;

public static class GamesUpdateEndpoints
{
    public static void MapGamesUpdateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games");

        group.MapPut("/{id:int}", async (int id, CreateGameDto input, AppDbContext db) =>
        {
            var game = await db.Games
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return Results.NotFound();

            game.Title = input.Title;
            game.Price = input.Price;
            game.ReleaseDate = input.ReleaseDate;

            game.Genres.Clear();

            if (input.GenreIds?.Any() == true)
            {
                var newGenres = await db.Genres
                    .Where(g => input.GenreIds.Contains(g.Id))
                    .ToListAsync();

                if (newGenres.Count != input.GenreIds.Count)
                    return Results.BadRequest("Um ou mais gêneros não existem");

                game.Genres.AddRange(newGenres);
            }

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateGame");
    }
}