using DDD.Domain;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.Database.Configurations;

public sealed class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        // Таблица
        builder.ToTable("buyers");

        // Ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_buyers");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => BuyerId.Create(fromDb).Value)
            .HasColumnName("id");

        // Ссылка на клиента
        builder.Property(x => x.ClientId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
            .HasColumnName("client_id");

        // Критерии поиска (опционально)
        builder.OwnsOne(x => x.SearchCriteria, criteriaBuilder =>
        {
            criteriaBuilder.WithOwner();

            criteriaBuilder.Property(c => c.PreferredNumberOfRooms)
                .HasConversion(toDb => toDb.Value, fromDb => NumberOfRooms.Create(fromDb).Value)
                .IsRequired()
                .HasColumnName("preferred_number_of_rooms");

            criteriaBuilder.Property(c => c.PreferredFloor)
                .HasConversion(
                    toDb => toDb.Value,
                    fromDb => Floor.Create(fromDb).Value)
                .IsRequired()
                .HasColumnName("preferred_floor");

            criteriaBuilder.Property(c => c.PreferredTotalFloors)
                .HasConversion(toDb => toDb.Value, fromDb => TotalFloors.Create(fromDb).Value)
                .IsRequired()
                .HasColumnName("preferred_total_floors");

            criteriaBuilder.Property(c => c.PreferredType)
                .HasConversion(v => v.Name, v => SmartPropertyType.FromName(v))
                .IsRequired()
                .HasColumnName("preferred_type");

            criteriaBuilder.Property(c => c.PreferParking)
                .HasColumnName("prefer_parking");

            criteriaBuilder.Property(c => c.PreferredHeatingType)
                .HasConversion(toDb => toDb.Value, fromDb => HeatingType.Create(fromDb).Value)
                .IsRequired()
                .HasColumnName("preferred_heating_type");

            criteriaBuilder.Property(c => c.PreferredCondition)
                .HasConversion(toDb => toDb.Value, fromDb => PropertyCondition.Create(fromDb).Value)
                .IsRequired()
                .HasColumnName("preferred_condition");
        });

        // Навигация SearchCriteria обязательна
        builder.Navigation(x => x.SearchCriteria).IsRequired();
    }
}
