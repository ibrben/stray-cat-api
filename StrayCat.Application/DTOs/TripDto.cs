using Microsoft.AspNetCore.Components.Routing;
using StrayCat.Domain.Enums;
using System.Text.Json.Serialization;

namespace StrayCat.Application.DTOs
{
    public class TripDto
    {
        [JsonPropertyName("tripId")]
        public int TripId { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonPropertyName("maxGuests")]
        public int MaxGuests { get; set; }
        
        [JsonPropertyName("price")]
        public int Price { get; set; }
        
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("type")]
        public TripType Type { get; set; } = TripType.Trip;
        
        [JsonPropertyName("category")]
        public TripCategory Category { get; set; }
        
        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }
        
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }
        
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();
        
        [JsonPropertyName("organizer")]
        public OrganizerDto Organizer { get; set; } = new();
        
        [JsonPropertyName("bookedGuest")]
        public int BookedGuest { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "THB";
    }

    public class OrganizerDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
