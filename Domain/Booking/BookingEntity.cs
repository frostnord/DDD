using CSharpFunctionalExtensions;
using Domain.Agency.VO;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;

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
    /// Идентификатор агентства, осуществляющего бронирование
    /// </summary>
    public AgencyId AgencyId { get; private set; }

    /// <summary>
    /// Период бронирования
    /// </summary>
    public Period BookingPeriod { get; private set; }

    /// <summary>
    /// Общая цена бронирования
    /// </summary>
    public Price TotalPrice { get; private set; }


    /// <summary>
    /// Дата создания бронирования
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления бронирования
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Создает новый экземпляр бронирования через фабричный метод
    /// </summary>
    /// <param name="clientId">Идентификатор клиента, совершающего бронирование</param>
    /// <param name="propertyId">Идентификатор объекта недвижимости, который бронируется</param>
    /// <param name="agencyId">Идентификатор агентства, осуществляющего бронирование</param>
    /// <param name="bookingPeriod">Период бронирования</param>
    /// <param name="totalPrice">Общая цена бронирования</param>
    /// <returns>Результат с бронированием или ошибкой</returns>
    public static Result<BookingEntity> Create(ClientId clientId, PropertyId propertyId, AgencyId agencyId,
        Period bookingPeriod, Price totalPrice)
    {
        var validationErrors = new List<string>();

        if (clientId == null)
            validationErrors.Add("Идентификатор клиента не может быть пустым");

        if (propertyId == null)
            validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

        if (agencyId == null)
            validationErrors.Add("Идентификатор агентства не может быть пустым");

        if (bookingPeriod == null)
            validationErrors.Add("Период бронирования не может быть пустым");

        if (totalPrice == null)
            validationErrors.Add("Общая цена не может быть пустой");

        if (validationErrors.Count > 0)
        {
            return Result.Failure<BookingEntity>(string.Join("; ", validationErrors));
        }

        var id = BookingId.Create(Guid.NewGuid()).Value;
        var booking = new BookingEntity(id, clientId, propertyId, agencyId, bookingPeriod, totalPrice);
        return Result.Success(booking);
    }

    /// <summary>
    /// Создает новый экземпляр бронирования
    /// </summary>
    /// <param name="id">Уникальный идентификатор бронирования</param>
    /// <param name="clientId">Идентификатор клиента, совершающего бронирование</param>
    /// <param name="propertyId">Идентификатор объекта недвижимости, который бронируется</param>
    /// <param name="agencyId">Идентификатор агентства, осуществляющего бронирование</param>
    /// <param name="bookingPeriod">Период бронирования</param>
    /// <param name="totalPrice">Общая цена бронирования</param>
    protected BookingEntity(BookingId id, ClientId clientId, PropertyId propertyId, AgencyId agencyId,
        Period bookingPeriod, Price totalPrice)
        : base(id)
    {
        ClientId = clientId;
        PropertyId = propertyId;
        AgencyId = agencyId;
        BookingPeriod = bookingPeriod;
        TotalPrice = totalPrice;
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
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Отменяет бронирование
    /// </summary>
    /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя отменить</exception>
    public void Cancel()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Завершает бронирование
    /// </summary>
    /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя завершить</exception>
    public void Complete()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
