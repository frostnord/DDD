using System;
using Domain.Domain.Booking.Booking.VO;
using Domain.Domain.Booking.VO;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Deal;
using Domain.Domain.Property.Property.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Deals;

public sealed class DealEntityConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        // Создаем таблицу
        builder.ToTable("deals");

        // Устанавливаем ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_deals");

        // Конфигурация ключа
        builder.Property(x => x.Id)
            .HasConversion(toDb => toDb.Value, fromDb => DealId.Create(fromDb).Value)
            .HasColumnName("id");

        // Конфигурация идентификаторов
        builder.Property(x => x.ClientId)
            .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
            .HasColumnName("client_id");

        builder.Property(x => x.PropertyId)
            .HasConversion(toDb => toDb.Value, fromDb => PropertyId.Create(fromDb).Value)
            .HasColumnName("property_id");

        builder.Property(x => x.BookingId)
            .HasConversion(
                toDb => toDb != null ? toDb.Value : Guid.Empty,
                fromDb => fromDb == Guid.Empty ? null : BookingId.Create(fromDb).Value)
            .HasColumnName("booking_id");

        // Настройка деталей сделки (составного значения)
        builder.OwnsOne(x => x.Details, detailsBuilder =>
        {
            detailsBuilder.Property(d => d.DealDate)
                .IsRequired()
                .HasColumnName("deal_date");

            detailsBuilder.Property(d => d.DealAmount)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => Price.Create(fromDb).Value)
                .HasColumnName("deal_amount");

            detailsBuilder.Property(d => d.DealType)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("deal_type");

            detailsBuilder.Property(d => d.Comments)
                .HasMaxLength(500)
                .HasColumnName("comments");
        });

        // Настройка статуса сделки
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion(
                v => v.Name,
                v => DealStatus.FromName(v))
            .HasColumnName("status");

        // Другие свойства
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("updated_at");

        // Настройка документов как отдельной таблицы
        builder.OwnsMany(x => x.Documents, documentBuilder =>
        {
            documentBuilder.ToTable("deal_documents");

            documentBuilder.Property<int>("Id").UseIdentityColumn().HasColumnName("id");
            documentBuilder.WithOwner().HasForeignKey("deal_id");

            documentBuilder.Property(d => d.Id)
                .HasColumnName("document_id");

            documentBuilder.Property(d => d.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("title");

            documentBuilder.Property(d => d.DocumentType)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("document_type");

            documentBuilder.Property(d => d.FilePath)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("file_path");

            documentBuilder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");
        });
    }
}
