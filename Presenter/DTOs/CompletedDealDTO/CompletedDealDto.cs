using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.CompletedDealDTO;

/// <summary>
/// DTO для представления завершенной сделки
/// </summary>
public class CompletedDealDto
{
    public Guid Id { get; set; }

    public Guid BuyerClientId { get; set; }

    public Guid SellerClientId { get; set; }

    [Required]
    public Guid PropertyId { get; set; }

    public DateTime DealDate { get; set; }

    public decimal DealAmount { get; set; }

    public string DealType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
