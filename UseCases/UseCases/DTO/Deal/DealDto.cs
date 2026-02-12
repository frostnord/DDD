using System;
using Domain.Deal;
using Domain.Deal.VO;

namespace UseCases.UseCases.DTO.Deal;

public sealed record DealDto(
    Guid Id,
    Guid ClientId,
    Guid PropertyId,
    DealDetails Details,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
