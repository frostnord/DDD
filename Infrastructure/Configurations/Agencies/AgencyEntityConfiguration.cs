using Domain.Domain;
using Domain.Domain.Agency;
using Domain.Domain.Agency.VO;
using Domain.Domain.ValueObjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Agencies;

public sealed class AgencyEntityConfiguration : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        // Таблица
        builder.ToTable("agency");

        // Ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_agency");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => AgencyId.Create(fromDb).Value)
            .HasColumnName("id");

        // Название агентства (VO Name)
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Name.MaxLength)
            .HasConversion(toDb => toDb.Value, fromDb => Name.Create(fromDb).Value)
            .HasColumnName("name");

        // Номер лицензии (VO LicenseNumber)
        builder.Property(x => x.LicenseNumber)
            .IsRequired()
            .HasMaxLength(LicenseNumber.MAX_LENGTH)
            .HasConversion(toDb => toDb.Value, fromDb => LicenseNumber.Create(fromDb).Value)
            .HasColumnName("license_number");

        // Временные метки
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("updated_at");

        // Контактная информация (owned type ContactInfo)
        builder.OwnsOne(x => x.ContactInfo, contactBuilder =>
        {
            contactBuilder.Property(c => c.Email)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => Email.Create(fromDb).Value)
                .HasMaxLength(Email.MAX_EMAIL_LEANGTH)
                .HasColumnName("email");

            contactBuilder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => PhoneNumber.Create(fromDb).Value)
                .HasMaxLength(PhoneNumber.MAX_TELEPHONE_NUMBER)
                .HasColumnName("phone_number");
        });

        // Пока не настраиваем связь с Property (агрегаты разделены) — игнорируем проекцию списка
        builder.Ignore(x => x.Properties);
    }
}
