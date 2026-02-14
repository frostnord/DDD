using System;

namespace Infrastructure;

public sealed class PostgreSqlConnectionOptions
{
    public required string Database { get; init; }
    public required string Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }

    public string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(Database))
            throw new ArgumentException("Database must be specified.", nameof(Database));

        if (string.IsNullOrWhiteSpace(Port))
            throw new ArgumentException("Port must be specified.", nameof(Port));

        if (string.IsNullOrWhiteSpace(Username))
            throw new ArgumentException("Username must be specified.", nameof(Username));

        if (string.IsNullOrWhiteSpace(Password))
            throw new ArgumentException("Password must be specified.", nameof(Password));

        return $"Host=localhost;Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}