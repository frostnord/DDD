using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;

namespace Domain.Booking;

/// <summary>
/// Агрегат бронирования в системе управления недвижимостью
/// </summary>
public class BookingEntity : Entity<BookingId>
{
    // Id уже определен в базовом классе CSharpFunctionalExtensions.Entity<TId>

    /// <summary>
    /// Идентификатор клиента, совершающего бронирование
    /// </summary>
    public ClientId ClientId { get; private set; }

    /// <summary>
    /// Идентификатор объекта недвижимости, который бронируется
    /// </summary>
    public PropertyId PropertyId { get; private set; }

    /// <summary>
    /// Дата начала hold-резервации
    /// </summary>
    public DateTime ReservedAt { get; private set; }

    /// <summary>
    /// Дата окончания hold-резервации
    /// </summary>
    public DateTime ReservedUntil { get; private set; }

    /// <summary>
    /// Статус бронирования (hold)
    /// </summary>
    public BookingStatus Status { get; private set; }


    /// <summary>
    /// Дата создания бронирования
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления бронирования
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Создает новый экземпляр hold-бронирования через фабричный метод
    /// </summary>
    /// <param name="clientId">Идентификатор клиента, совершающего бронирование</param>
    /// <param name="propertyId">Идентификатор объекта недвижимости, который бронируется</param>
    /// <param name="reservedUntil">Дата окончания hold-резервации</param>
    /// <returns>Результат с бронированием или ошибкой</returns>
    public static Result<BookingEntity> CreateHold(ClientId clientId, PropertyId propertyId,
        DateTime reservedUntil)
    {
        var validationErrors = new List<string>();

        if (clientId == null)
            validationErrors.Add("Идентификатор клиента не может быть пустым");

        if (propertyId == null)
            validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

        if (reservedUntil == default(DateTime))
            validationErrors.Add("Дата окончания бронирования не может быть пустой");

        if (reservedUntil <= DateTime.UtcNow)
            validationErrors.Add("Дата окончания бронирования должна быть в будущем");

        if (validationErrors.Count > 0)
        {
            return Result.Failure<BookingEntity>(string.Join("; ", validationErrors));
        }

        var id = BookingId.Create(Guid.NewGuid()).Value;
        var reservedAt = DateTime.UtcNow;
        var booking = new BookingEntity(id, clientId, propertyId, reservedAt, reservedUntil);
        return Result.Success(booking);
    }

    /// <summary>
    /// Создает новый экземпляр бронирования
    /// </summary>
    /// <param name="id">Уникальный идентификатор бронирования</param>
    /// <param name="clientId">Идентификатор клиента, совершающего бронирование</param>
    /// <param name="propertyId">Идентификатор объекта недвижимости, который бронируется</param>
    /// <param name="reservedAt">Дата начала hold-резервации</param>
    /// <param name="reservedUntil">Дата окончания hold-резервации</param>
    protected BookingEntity(BookingId id, ClientId clientId, PropertyId propertyId,
        DateTime reservedAt, DateTime reservedUntil)
        : base(id)
    {
        ClientId = clientId;
        PropertyId = propertyId;
        ReservedAt = reservedAt;
        ReservedUntil = reservedUntil;
        Status = BookingStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
    
    // EF Core конструктор
    protected BookingEntity()
    {
    }

    /// <summary>
    /// Подтверждает бронирование
    /// </summary>
    /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя подтвердить</exception>
    public void Confirm()
    {
        if (Status != BookingStatus.Active)
        {
            throw new InvalidOperationException($"Нельзя подтвердить бронирование в статусе '{Status.Name}'");
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Отменяет бронирование
    /// </summary>
    /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя отменить</exception>
    public void Cancel()
    {
        if (Status != BookingStatus.Active)
        {
            throw new InvalidOperationException($"Нельзя отменить бронирование в статусе '{Status.Name}'");
        }

        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Завершает бронирование
    /// </summary>
    /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя завершить</exception>
    public void Complete()
    {
        if (Status != BookingStatus.Active)
        {
            throw new InvalidOperationException($"Нельзя завершить бронирование в статусе '{Status.Name}'");
        }

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        if (Status != BookingStatus.Active)
        {
            throw new InvalidOperationException($"Нельзя истечь бронирование в статусе '{Status.Name}'");
        }

        Status = BookingStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }
}
