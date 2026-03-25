using System.Text.Json;
using System.Text.Json.Serialization;
namespace GameLibraryApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Game> Games { get; set; } = new();      
    }
}
