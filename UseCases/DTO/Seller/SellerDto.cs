using System;

namespace UseCases.DTO.Seller;

/// <summary>
/// DTO для представления продавца
/// </summary>
public record SellerDto(
    Guid Id,
    Guid ClientId,
    DateTime RegisteredAt
);