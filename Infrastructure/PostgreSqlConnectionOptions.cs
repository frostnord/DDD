namespace Infrastructure;

public sealed class PostgreSqlConnectionOptions
{

    public required string Database { get; init; }
    public required string Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }

    public string GetConnectionString()
    {
        return $"Host=localhost;Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}