using System.ComponentModel.DataAnnotations.Schema;
using StrayCat.Domain.Enums;

namespace StrayCat.Domain.Entities
{
    [Table("trips")]
    public class Trip
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TripCategory Category { get; set; } = TripCategory.Adventure;
        public string Location { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int MaxGuests { get; set; }
        public TripType Type { get; set; } = TripType.Trip;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int OrganizerId { get; set; }
        public Organizer Organizer { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<TripDate> TripDates { get; set; } = new List<TripDate>();
        public ICollection<TripTag> TripTags { get; set; } = new List<TripTag>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<TripImage> TripImages { get; set; } = new List<TripImage>();
        public ICollection<Highlight> Highlights { get; set; } = new List<Highlight>();
    }
}
