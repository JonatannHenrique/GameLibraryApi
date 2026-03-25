namespace GameLibraryApi.Dtos;

public class CreateGameDto
{
    public string Title { get; set; } = default!;
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public List<int> GenreIds { get; set; } = new();  
}