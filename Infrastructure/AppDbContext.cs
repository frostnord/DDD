using Domain.Domain;
using Domain.Domain.Booking;
using Domain.Domain.Booking.Booking;
using Domain.Domain.Customers.Buyer;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Seller;
using Domain.Domain.Deal;
using Domain.Domain.Property;
using Domain.Domain.Property.Property;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    // DbSets (агрегаты)
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<CompletedDeal> CompletedDeals => Set<CompletedDeal>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Подключаем все IEntityTypeConfiguration из сборки DbContext
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
