using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrayCat.Domain.Entities
{
    [Table("trip_images")]
    public class TripImage
    {
        public int Id { get; set; }
        
        public int TripId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property for Trip
        public Trip Trip { get; set; } = null!;
    }
}
