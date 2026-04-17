using System.ComponentModel.DataAnnotations.Schema;

namespace StrayCat.Domain.Entities
{
    [Table("highlights")]
    public class Highlight
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string Item { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public Trip Trip { get; set; } = null!;
    }
}
