using StrayCat.Domain.Enums;

namespace StrayCat.Application.DTOs
{
    public class TripDto
    {
        public int TripId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxGuests { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public TripType Type { get; set; } = TripType.Trip;
        public TripCategory Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public OrganizerDto Organizer { get; set; } = new();
        public int BookedGuest { get; set; }
        
    }

    public class OrganizerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
