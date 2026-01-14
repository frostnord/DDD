using System.Collections.Generic;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Booking.Queries.SearchBookingsQuery
{
    public record SearchBookingsQueryResponse(List<BookingDto> Items);
}
