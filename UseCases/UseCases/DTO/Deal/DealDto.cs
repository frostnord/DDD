using System;
using Domain.Deal;

namespace UseCases.UseCases.DTO.Deal;

public sealed record DealDto(
    Guid Id,
    Guid ClientId,
    Guid PropertyId,
    Guid? BookingId,
    DealDetails Details,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
