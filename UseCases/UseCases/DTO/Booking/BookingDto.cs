using System;

namespace UseCases.UseCases.DTO.Booking
{
    public record BookingDto(
        Guid Id,
        Guid ClientId,
        Guid PropertyId,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
