using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Presenter;

public static class InfrastructureInjection
{
    public static void AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}
