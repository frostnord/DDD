using Domain.Booking;
using Domain.Customers.Buyer;
using Domain.Customers.Client;
using Domain.Customers.Seller;
using Domain.Deal;
using Domain.Property;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets (агрегаты)
    public DbSet<PropertyEntity> Properties => Set<PropertyEntity>();
    public DbSet<BuyerEntity> Buyers => Set<BuyerEntity>();
    public DbSet<SellerEntity> Sellers => Set<SellerEntity>();
    public DbSet<CompletedDealEntity> CompletedDeals => Set<CompletedDealEntity>();
    public DbSet<DealEntity> Deals => Set<DealEntity>();
    public DbSet<ClientEntity> Clients => Set<ClientEntity>();
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Подключаем все IEntityTypeConfiguration из сборки DbContext
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}