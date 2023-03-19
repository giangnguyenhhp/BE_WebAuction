namespace Entities.Identity.AuthenticateRequestModels;

public class AuthResponse
{
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}