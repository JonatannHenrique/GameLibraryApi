using GameLibraryApi.Data;
using GameLibraryApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=games.db"));

builder.Services.AddCors(opt => opt.AddPolicy("all", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("all");

app.MapGamesListEndpoints();
app.MapGamesCreateEndpoints();
app.MapGamesUpdateEndpoints();
app.MapGamesDeleteEndpoints();

// Migração do banco
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();