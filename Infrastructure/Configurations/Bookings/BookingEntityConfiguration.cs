using Domain.Agency.VO;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Bookings;

public sealed class BookingEntityConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        // Таблица
        builder.ToTable("booking");

        // Ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_booking");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => BookingId.Create(fromDb).Value)
            .HasColumnName("id");

        // Временные метки
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("updated_at");

        // Общая цена бронирования (VO Price)
        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => Price.Create(fromDb).Value)
            .HasColumnName("total_price");

        // Период бронирования (owned type Period)
        builder.OwnsOne(x => x.BookingPeriod, periodBuilder =>
        {
            periodBuilder.Property(p => p.StartDate)
                .IsRequired()
                .HasColumnName("start_date");

            periodBuilder.Property(p => p.EndDate)
                .IsRequired()
                .HasColumnName("end_date");
        });

        // Ссылки на агрегаты (ограничиваем каскадное удаление)
        builder.Property(x => x.ClientId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
            .HasColumnName("client_id");

        builder.Property(x => x.PropertyId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => PropertyId.Create(fromDb).Value)
            .HasColumnName("property_id");

        builder.Property(x => x.AgencyId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => AgencyId.Create(fromDb).Value)
            .HasColumnName("agency_id");
    }
}