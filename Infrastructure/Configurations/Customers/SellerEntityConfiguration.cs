using Domain.Domain;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Customers.Seller.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.DbContext.Database.Configurations;

public sealed class SellerEntityConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        // Таблица
        builder.ToTable("sellers");

        // Ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_sellers");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => SellerId.Create(fromDb).Value)
            .HasColumnName("id");

        // Ссылка на клиента
        builder.Property(x => x.ClientId)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
            .HasColumnName("client_id");

        //  список принадлежащих продавцу объектов (PropertyId)
        builder.OwnsMany<PropertyId>("_ownedProperties", nav =>
        {
            nav.ToTable("seller_properties");

            // FK на продавца
            nav.WithOwner().HasForeignKey("seller_id");

            // Значение PropertyId хранится как GUID
            nav.Property(p => p.Value)
                .IsRequired()
                .HasColumnName("property_id");

            // Составной ключ для уникальности
            nav.HasKey("seller_id", "property_id");
        });
    }
}
