using System.ComponentModel.DataAnnotations.Schema;
using StrayCat.Domain.Entities;

namespace StrayCat.Domain.Entities
{
    [Table("trip_dates")]
    public class TripDate
    {
        public int Id { get; set; }
        
        public int TripId { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public Trip Trip { get; set; } = null!;
    }
}
