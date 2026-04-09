using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrayCat.Domain.Entities
{
    [Table("bookings")]
    public class Booking
    {
        public int Id { get; set; }
        
        public int TripId { get; set; }
        
        [Required]
        [StringLength(7)]
        [MinLength(7)]
        [RegularExpression(@"^[A-Za-z0-9]{7}$", ErrorMessage = "Reference code must be exactly 7 alphanumeric characters")]
        public string ReferenceCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string CustomerPhoneNo { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue, ErrorMessage = "Guest count must be at least 1")]
        public int GuestCount { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "Total price cannot be negative")]
        public decimal TotalPrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Service fee cannot be negative")]
        public decimal ServiceFee { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Grand total cannot be negative")]
        public decimal GrandTotal { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property for Trip
        public Trip Trip { get; set; } = null!;
    }
}
