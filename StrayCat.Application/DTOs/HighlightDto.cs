using System.Text.Json.Serialization;

namespace StrayCat.Application.DTOs
{
    public class HighlightDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("tripId")]
        public int TripId { get; set; }
        
        [JsonPropertyName("item")]
        public string Item { get; set; } = string.Empty;
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateHighlightRequestDto
    {
        [JsonPropertyName("tripId")]
        public int TripId { get; set; }
        
        [JsonPropertyName("items")]
        public List<string> Items { get; set; } = new();
    }

    public class CreateHighlightResponseDto
    {
        [JsonPropertyName("highlights")]
        public List<HighlightDto> Highlights { get; set; } = new();
        
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
