using GameLibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Genres)
            .WithMany(gen => gen.Games);


     

    }
}