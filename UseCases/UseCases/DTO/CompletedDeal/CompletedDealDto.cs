using System;

namespace UseCases.UseCases.DTO.CompletedDeal;

public sealed record CompletedDealDto(
    Guid Id,
    Guid BuyerClientId,
    Guid SellerClientId,
    Guid PropertyId,
    DateTime DealDate,
    decimal DealAmount,
    string DealType,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
