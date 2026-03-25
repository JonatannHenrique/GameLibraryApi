using GameLibraryApi.Data;
using GameLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryApi.Endpoints;

public static class GamesDeleteEndpoints
{
    public static void MapGamesDeleteEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games");

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var game = await db.Games.FindAsync(id);

            if (game == null)
                return Results.NotFound();

            db.Games.Remove(game);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteGame");
    }
}