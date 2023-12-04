namespace Tradibit.Shared.DTO.Auth;

public class AuthConfig
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public int TokenExpireSeconds { get; set; }
    public string? JwtSecret { get; set; }
}