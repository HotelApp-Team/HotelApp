namespace HotelApp.Domain.DTOs;

public class RegisterResponseDto
{
    public bool RequiresEmailVerification { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
