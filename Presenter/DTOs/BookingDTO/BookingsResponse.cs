using System.Collections.Generic;

namespace Presenter.DTOs.BookingDTO
{
    public record BookingsResponse(
        IEnumerable<BookingDto> Items
    );
}
