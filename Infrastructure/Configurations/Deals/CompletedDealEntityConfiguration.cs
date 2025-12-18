using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Deals;

public sealed class CompletedDealEntityConfiguration : IEntityTypeConfiguration<CompletedDealEntity>
{
    public void Configure(EntityTypeBuilder<CompletedDealEntity> builder)
    {
        builder.ToTable("completed_deals");

        builder.HasKey(x => x.Id)
            .HasName("pk_completed_deals");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(v => v.Value, v => CompletedDealId.Create(v).Value)
            .HasColumnName("id");

        builder.Property(x => x.BuyerClientId)
            .IsRequired()
            .HasConversion(v => v.Value, v => ClientId.Create(v).Value)
            .HasColumnName("buyer_client_id");

        builder.Property(x => x.SellerClientId)
            .IsRequired()
            .HasConversion(v => v.Value, v => ClientId.Create(v).Value)
            .HasColumnName("seller_client_id");

        builder.Property(x => x.PropertyId)
            .IsRequired()
            .HasConversion(v => v.Value, v => PropertyId.Create(v).Value)
            .HasColumnName("property_id");

        builder.Property(x => x.DealAmount)
            .IsRequired()
            .HasConversion(v => v.Value, v => Price.Create(v).Value)
            .HasColumnName("deal_amount");

        builder.Property(x => x.DealDate)
            .IsRequired()
            .HasColumnName("deal_date");

        builder.Property(x => x.DealType)
            .IsRequired()
            .HasConversion(v => v.Name, v => DealType.FromName(v))
            .HasMaxLength(DealType.MAX_DEAL_TYPE_LENGTH)
            .HasColumnName("deal_type");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("updated_at");
    }
}