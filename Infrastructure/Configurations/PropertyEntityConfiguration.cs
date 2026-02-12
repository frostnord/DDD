using Domain.Customers.Client.VO;
using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public sealed class PropertyEntityConfiguration : IEntityTypeConfiguration<PropertyEntity>
{
    public void Configure(EntityTypeBuilder<PropertyEntity> builder)
    {
        // Создаем таблицу
        builder.ToTable("property");

        // Устанавливаем ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_property");

        // Конфигурация ключа
        builder.Property(x => x.Id)
            .HasConversion(toDb => toDb.Value, fromDb => PropertyId.Create(fromDb).Value)
            .HasColumnName("id");

        // Конфигурация свойств
        builder.Property(x => x.Price)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => Price.Create(fromDb).Value)
            .HasColumnName("price");

        builder.Property(x => x.Description)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => Description.Create(fromDb).Value)
            .HasMaxLength(Description.MAX_LENGTH)
            .HasColumnName("description");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // Конфигурация статуса недвижимости (smart-enum)
        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion(
                v => v.Name,
                v => PropertyStatus.FromName(v))
            .HasMaxLength(PropertyStatus.MAX_STATUS_LENGTH);

        builder.Property(x => x.ReservedByClientId)
            .HasConversion(
                toDb => toDb == null ? (Guid?)null : toDb.Value,
                fromDb => fromDb == null ? null : ClientId.Create(fromDb.Value).Value)
            .HasColumnName("reserved_by_client_id");

        builder.Property(x => x.ReservedAt)
            .HasColumnName("reserved_at");

        builder.Property(x => x.ReservedUntil)
            .HasColumnName("reserved_until");

        // Настройка свойства Address (составного значения)
        builder.OwnsOne(x => x.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(Address.MAX_STREET_LENGTH)
                .HasColumnName("street");

            addressBuilder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(Address.MAX_CITY_LENGTH)
                .HasColumnName("city");

            addressBuilder.Property(a => a.HomeNumber)
                .IsRequired()
                .HasColumnName("home_number");

            addressBuilder.Property(a => a.ZipCode)
                .IsRequired()
                .HasColumnName("zip_code");

            addressBuilder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("country");
        });

        // Настройка свойства Details (составного значения)
        builder.OwnsOne(x => x.PropertyDetails, detailsBuilder =>
        {
            detailsBuilder.Property(d => d.Area)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => Area.Create(fromDb).Value)
                .HasColumnName("area");
            detailsBuilder.Property(d => d.NumberOfRooms)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => NumberOfRooms.Create(fromDb).Value)
                .HasColumnName("number_of_rooms");

            detailsBuilder.Property(d => d.Floor)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => Floor.Create(fromDb).Value)
                .HasColumnName("floor");
            detailsBuilder.Property(d => d.TotalFloors)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => TotalFloors.Create(fromDb).Value)
                .HasColumnName("total_floors");
            detailsBuilder.Property(d => d.Type)
                .IsRequired()
                .HasColumnName("property_type")
                .HasConversion(
                    v => v.Name,
                    v => SmartPropertyType.FromName(v));
            detailsBuilder.Property(d => d.HasBalcony)
                .IsRequired()
                .HasColumnName("has_balcony");

            detailsBuilder.Property(d => d.HasParking)
                .IsRequired()
                .HasColumnName("has_parking");

            detailsBuilder.Property(d => d.HeatingType)
                .IsRequired()
                .HasConversion(
                    toDb => toDb.Name,
                    fromDb => HeatingType.FromName(fromDb))
                .HasMaxLength(20)
                .HasColumnName("heating_type");

            detailsBuilder.Property(d => d.Condition)
                .HasConversion(toDb => toDb.Value, fromDb => PropertyCondition.Create(fromDb).Value)
                .HasColumnName("condition");
        });
        builder.OwnsOne<OwnershipHistory>("_ownershipHistory", historyBuilder =>
        {
            historyBuilder.WithOwner();
            historyBuilder.OwnsMany<OwnershipRecord>("Records", ownershipBuilder =>
            {
                ownershipBuilder.ToTable("property_ownership_history");
                ownershipBuilder.Property<int>("Id").UseIdentityColumn().HasColumnName("id");
                ownershipBuilder.WithOwner().HasForeignKey("property_id");

                ownershipBuilder.Property(oh => oh.OwnerClientId)
                    .IsRequired()
                    .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
                    .HasColumnName("client_id");

                ownershipBuilder.Property(oh => oh.StartDate)
                    .IsRequired()
                    .HasColumnName("start_date");

                ownershipBuilder.Property(oh => oh.EndDate)
                    .IsRequired(false)
                    .HasColumnName("end_date");
            });
        });

        builder.Navigation("_ownershipHistory").IsRequired();
    }
}
