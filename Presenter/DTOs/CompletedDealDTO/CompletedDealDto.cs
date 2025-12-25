using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.CompletedDealDTO;

/// <summary>
/// DTO для представления завершенной сделки
/// </summary>
public record CompletedDealDto(
    Guid Id,
    Guid BuyerClientId,
    Guid SellerClientId,
    Guid PropertyId,
    DateTime DealDate,
    decimal DealAmount,
    string DealType,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
