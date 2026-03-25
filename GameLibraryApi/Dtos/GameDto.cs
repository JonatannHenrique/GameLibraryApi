namespace GameLibraryApi.Dtos;

public class GameDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public List<string> Genres { get; set; } = new();  
}