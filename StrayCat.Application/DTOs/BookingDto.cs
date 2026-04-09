namespace StrayCat.Application.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string ReferenceCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNo { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Trip details for response
        public TripSummaryDto? Trip { get; set; }
    }

    public class CreateBookingDto
    {
        public int TripId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNo { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal ServiceFee { get; set; }
    }

    public class TripSummaryDto
    {
        public int TripId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
