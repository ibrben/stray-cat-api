using System.ComponentModel.DataAnnotations.Schema;
using StrayCat.Domain.Entities;

namespace StrayCat.Domain.Entities
{
    [Table("trip_tags")]
    public class TripTag
    {
        public int Id { get; set; }
        
        public int TripId { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public Trip Trip { get; set; } = null!;
    }
}
