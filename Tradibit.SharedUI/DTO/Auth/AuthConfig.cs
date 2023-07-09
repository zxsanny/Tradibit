namespace Tradibit.SharedUI.DTO.Auth;

public class AuthConfig
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Key { get; set; }
    public int TokenExpireSeconds { get; set; }
}