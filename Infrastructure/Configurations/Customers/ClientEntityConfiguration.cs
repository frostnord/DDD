using Domain.Domain;
using Domain.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.DbContext.Database.Configurations;

public sealed class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Создаем таблицу
        builder.ToTable("clients");

        // Устанавливаем ключ
        builder.HasKey(x => x.Id)
            .HasName("pk_clients");

        // Конфигурация ключа
        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => ClientId.Create(fromDb).Value)
            .HasColumnName("id");

        // Конфигурация свойств имени
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => Name.Create(fromDb).Value)
            .HasColumnName("first_name");

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => Name.Create(fromDb).Value)
            .HasColumnName("last_name");

        // Настройка контактной информации (составного значения)
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

        // Другие свойства
        builder.Property(x => x.RegisteredDate)
            .IsRequired()
            .HasColumnName("registered_date");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("updated_at");
    }
}