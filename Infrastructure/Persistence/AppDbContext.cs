using Domain.Domain;
using Domain.Domain.Deal;
using Microsoft.EntityFrameworkCore;

namespace Domain.Database;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    // DbSets (агрегаты)
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<CompletedDeal> CompletedDeals => Set<CompletedDeal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Подключаем все IEntityTypeConfiguration из сборки DbContext
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
