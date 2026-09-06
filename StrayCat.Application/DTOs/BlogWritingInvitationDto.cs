using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StrayCat.Application.DTOs
{
    public class CreateBlogWritingInvitationDto
    {
        [Required]
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        
        [Required]
        [Range(1, 20)]
        [JsonPropertyName("maxSubmissions")]
        public int MaxSubmissions { get; set; }
        
        [JsonPropertyName("tripId")]
        public int? TripId { get; set; }
    }

    public class BlogWritingInvitationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("writingUrl")]
        public string? WritingUrl { get; set; }
        
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        
        [JsonPropertyName("maxSubmissions")]
        public int MaxSubmissions { get; set; }
        
        [JsonPropertyName("submissionCount")]
        public int SubmissionCount { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("tripId")]
        public int? TripId { get; set; }
    }

    public class GuestBlogWritingContextDto
    {
        [JsonPropertyName("inviterDisplayName")]
        public string? InviterDisplayName { get; set; }
        
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        
        [JsonPropertyName("submissionsRemaining")]
        public int SubmissionsRemaining { get; set; }
        
        [JsonPropertyName("trip")]
        public GuestBlogTripContextDto? Trip { get; set; }
    }

    public class GuestBlogTripContextDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class GuestBlogSubmissionDto
    {
        [Required]
        [StringLength(200)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(120)]
        [JsonPropertyName("publisherName")]
        public string PublisherName { get; set; } = string.Empty;
        
        [StringLength(500)]
        [JsonPropertyName("excerpt")]
        public string? Excerpt { get; set; }
        
        [StringLength(500)]
        [JsonPropertyName("featuredImageUrl")]
        public string? FeaturedImageUrl { get; set; }
    }

    public class GuestBlogSubmissionResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonPropertyName("publisherName")]
        public string PublisherName { get; set; } = string.Empty;
    }
}