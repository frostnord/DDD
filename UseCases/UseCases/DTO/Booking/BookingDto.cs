using System;

namespace UseCases.UseCases.DTO.Booking;

public record BookingDto(
    Guid Id,
    Guid ClientId,
    Guid PropertyId,
    DateTime ReservedAt,
    DateTime ReservedUntil,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
