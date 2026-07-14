using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrayCat.Domain.Entities
{
    [Table("blog_images")]
    public class BlogImage
    {
        public int Id { get; set; }
        
        public int BlogId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property for Blog
        public Blog Blog { get; set; } = null!;
    }
}
