namespace DesafioNEGOCIE.Infrastructure.Persistence;

public class ConnectionSettings
{
    public const string ConnectionSettingsName = "ConnectionSettings";
    public string? ConnectionString { get; set; } = null;
}