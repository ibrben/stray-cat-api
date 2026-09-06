using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StrayCat.Domain.Enums;

namespace StrayCat.Domain.Entities
{
    [Table("blogs")]
    public class Blog
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? FeaturedImageUrl { get; set; }
        
        [StringLength(200)]
        public string? Slug { get; set; }
        
        public bool IsPublished { get; set; }
        
        public DateTime? PublishedAt { get; set; }
        
        public int? TripId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // New fields for guest blog writing
        [StringLength(120)]
        public string? PublisherName { get; set; }
        
        [StringLength(50)]
        public string? SourceInviteId { get; set; }
        
        public PublicationStatus PublicationStatus { get; set; } = PublicationStatus.Draft;
        
        [StringLength(500)]
        public string? Excerpt { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        // Navigation properties
        public Trip? Trip { get; set; }
        public List<BlogImage> BlogImages { get; set; } = new();
    }
}
