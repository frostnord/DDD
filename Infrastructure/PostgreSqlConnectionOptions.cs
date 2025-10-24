namespace Infrastructure;

public sealed class PostgreSqlConnectionOptions
{
    public required string HostName { get; init; }
    public required string DatabaseName { get; init; }
    // public required string Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    
    public string GetConnectionString()
    {
        return $"Host={HostName};Database={DatabaseName};Username={Username};Password={Password}";
    }
}