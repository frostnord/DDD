using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects.CommonVO;

namespace DDD.Domain.ValueObjects.BookingVO;

/// <summary>
/// Идентификатор бронирования
/// </summary>
public class BookingId : TypedId<BookingId>
{
    private BookingId(Guid value) : base(value) { }

    public static Result<BookingId> Create(Guid value)
        => TypedId<BookingId>.Create(value, v => new BookingId(v));

    public static BookingId New() => new(Guid.NewGuid());
}