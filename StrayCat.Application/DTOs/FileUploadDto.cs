namespace StrayCat.Application.DTOs
{
    public class FileUploadResponseDto
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class PresignedUrlRequestDto
    {
        public int TripId { get; set; }
        public string FileName { get; set; } = string.Empty;
    }

    public class PresignedUrlResponseDto
    {
        public string FileName { get; set; } = string.Empty;
        public string PresignedUrl { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }

    public class UploadTripImageDto
    {
        public int TripId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UploadTripImageResponseDto
    {
        public TripImageDto Image { get; set; } = new();
        public FileUploadResponseDto File { get; set; } = new();
    }

    public class ConfirmUploadDto
    {
        public int TripId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
