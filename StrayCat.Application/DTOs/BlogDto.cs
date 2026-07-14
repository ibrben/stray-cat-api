using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using StrayCat.Application.DTOs;

namespace StrayCat.Application.DTOs
{
    public class BlogDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
        
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
        
        [JsonPropertyName("featuredImageUrl")]
        public string? FeaturedImageUrl { get; set; }
        
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }
        
        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }
        
        [JsonPropertyName("publishedAt")]
        public DateTime? PublishedAt { get; set; }
        
        [JsonPropertyName("tripId")]
        public int? TripId { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonPropertyName("trip")]
        public TripSummaryDto? Trip { get; set; }
    }

    public class CreateBlogDto
    {
        [Required]
        [StringLength(200)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(500)]
        [JsonPropertyName("featuredImageUrl")]
        public string? FeaturedImageUrl { get; set; }
        
        [StringLength(200)]
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }
        
        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; } = false;
        
        [JsonPropertyName("tripId")]
        public int? TripId { get; set; }
    }

    public class UpdateBlogDto
    {
        [Required]
        [StringLength(200)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(500)]
        [JsonPropertyName("featuredImageUrl")]
        public string? FeaturedImageUrl { get; set; }
        
        [StringLength(200)]
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }
        
        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }
        
        [JsonPropertyName("tripId")]
        public int? TripId { get; set; }
    }
}
