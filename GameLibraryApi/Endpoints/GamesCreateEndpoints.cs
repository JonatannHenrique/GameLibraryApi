using GameLibraryApi.Data;
using GameLibraryApi.Dtos;
using GameLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryApi.Endpoints;

public static class GamesCreateEndpoints
{
    public static void MapGamesCreateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games");

        group.MapPost("/", async (CreateGameDto input, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                return Results.BadRequest("Título é obrigatório");

            if (input.Price < 0)
                return Results.BadRequest("Preço não pode ser negativo");

            var game = new Game
            {
                Title = input.Title,
                Price = input.Price,
                ReleaseDate = input.ReleaseDate
            };

            if (input.GenreIds?.Any() == true)
            {
                var genres = await db.Genres
                    .Where(g => input.GenreIds.Contains(g.Id))
                    .ToListAsync();

                if (genres.Count != input.GenreIds.Count)
                    return Results.BadRequest("Um ou mais gêneros não existem");

                game.Genres.AddRange(genres);
            }

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var createdDto = new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                Genres = game.Genres.Select(gen => gen.Name).ToList()
            };

            return Results.Created($"/games/{game.Id}", createdDto);
        })
        .WithName("CreateGame");
    }
}