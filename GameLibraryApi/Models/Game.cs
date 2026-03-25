namespace GameLibraryApi.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateOnly ReleaseDate { get; set; } 

        public List<Genre> Genres { get; set; } = new();
    }
}
