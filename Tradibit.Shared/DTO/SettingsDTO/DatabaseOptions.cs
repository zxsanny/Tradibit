namespace Tradibit.Shared.DTO.SettingsDTO;

public class DatabaseOptions
{
    public int TimeoutSeconds { get; set; }
    public string? ConnectionString { get; set; }
}