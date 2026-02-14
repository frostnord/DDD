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
        DateTime ReservedAt,
        DateTime ReservedUntil,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}