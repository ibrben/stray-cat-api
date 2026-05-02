using System.Text.Json.Serialization;

namespace StrayCat.Application.DTOs
{
    public class PublishTripRequestDto
    {
        [JsonPropertyName("tripId")]
        public int TripId { get; set; }
        
        [JsonPropertyName("state")]
        public bool State { get; set; }
    }

    public class PublishTripResponseDto
    {
        [JsonPropertyName("tripId")]
        public int TripId { get; set; }
        
        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
