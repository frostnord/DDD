using CSharpFunctionalExtensions;


namespace DDD.Domain.ValueObjects;

/// <summary>
/// Идентификатор бронирования
/// </summary>
public class BookingId : TypedId<BookingId>
{
    private BookingId(Guid value) : base(value) { }

    public static Result<BookingId> Create(Guid value)
        => TypedId<BookingId>.Create(value, v => new BookingId(v));
}