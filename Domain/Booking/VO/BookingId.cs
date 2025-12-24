
using System;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Booking.VO;

/// <summary>
/// Идентификатор бронирования
/// </summary>
public class BookingId : TypedId<BookingId>
{
    private BookingId(Guid value) : base(value)
    {
    }

    public static Result<BookingId> Create(Guid value)
        => Create(value, v => new BookingId(v));
}