using System.Text.Json.Serialization;

namespace StrayCat.Application.DTOs;

public class ContactUsDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("category")]
    public ContactUsCategory Category { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
}
public class ContactUsResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
public enum ContactUsCategory
{
    General,
    HostRegistration,
    Partnership,
    PrivateBooking
}