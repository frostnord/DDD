using DDD.Domain;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.DbContext.Database.Configurations;

public sealed class BookingEntityConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
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
        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey("client_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey("property_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Agency)
            .WithMany()
            .HasForeignKey("agency_id")
            .OnDelete(DeleteBehavior.Restrict);

    }
}
