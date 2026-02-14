using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public sealed class DealEntityConfiguration : IEntityTypeConfiguration<DealEntity>
{
    public void Configure(EntityTypeBuilder<DealEntity> builder)
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
            .HasColumnName("updated_at");
    }
}