using System;

namespace Presenter.DTOs.BookingDTO
{
    /// <summary>
    /// DTO для бронирования
    /// </summary>
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