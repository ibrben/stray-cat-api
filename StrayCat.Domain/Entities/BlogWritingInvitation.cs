using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrayCat.Domain.Entities
{
    [Table("blog_writing_invitations")]
    public class BlogWritingInvitation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = string.Empty;
        
        public int OwnerUserId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string TokenHash { get; set; } = string.Empty;
        
        [Required]
        public DateTime ExpiresAt { get; set; }
        
        [Required]
        [Range(1, 20)]
        public int MaxSubmissions { get; set; }
        
        public int SubmissionCount { get; set; } = 0;
        
        public int? TripId { get; set; }
        
        public DateTime? RevokedAt { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public Organizer? Owner { get; set; }
        public Trip? Trip { get; set; }
    }
}