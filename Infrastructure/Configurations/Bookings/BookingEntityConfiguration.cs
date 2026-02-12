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
            .HasColumnName("updated_at");

        builder.Property(x => x.ReservedAt)
            .IsRequired()
            .HasColumnName("reserved_at");

        builder.Property(x => x.ReservedUntil)
            .IsRequired()
            .HasColumnName("reserved_until");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion(
                v => v.Name,
                v => BookingStatus.FromName(v))
            .HasMaxLength(BookingStatus.MAX_STATUS_LENGTH);

        // Ссылки на агрегаты (ограничиваем каскадное удаление)
        builder.Property(x => x.ClientId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
            .HasColumnName("client_id");

        builder.Property(x => x.PropertyId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => PropertyId.Create(fromDb).Value)
            .HasColumnName("property_id");
    }
}