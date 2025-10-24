using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

PostgreSqlConnectionOptions? options = builder.Configuration.GetSection(nameof(PostgreSqlConnectionOptions))
    .Get<PostgreSqlConnectionOptions>();

if (options == null)
{
    throw new InvalidOperationException("PostgreSqlConnectionOptions not found");
}

Console.WriteLine(options.HostName);
Console.WriteLine(options.DatabaseName);
Console.WriteLine(options.Username);
Console.WriteLine(options.Password);

var app = builder.Build();

app.Run();