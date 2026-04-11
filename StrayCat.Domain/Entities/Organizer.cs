using System.ComponentModel.DataAnnotations.Schema;

namespace StrayCat.Domain.Entities
{
    [Table("organizers")]
    public class Organizer
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string AvatarInitial { get; set; } = string.Empty;
        
        public bool IsVerified { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool InviteTokenUsed { get; set; }
        
        // Google OAuth properties
        public string? GoogleId { get; set; }
        
        public string? ProfilePictureUrl { get; set; }
        
        public string? MobilePhone { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property for Trips
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
