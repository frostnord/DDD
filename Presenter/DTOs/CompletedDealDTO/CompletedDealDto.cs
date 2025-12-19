using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.CompletedDealDTO;

/// <summary>
/// DTO для представления завершенной сделки
/// </summary>
public class CompletedDealDto
{
    /// <summary>
    /// Уникальный идентификатор завершенной сделки
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор клиента-покупателя
    /// </summary>
    [Required]
    public Guid BuyerClientId { get; set; }

    /// <summary>
    /// Идентификатор клиента-продавца
    /// </summary>
    [Required]
    public Guid SellerClientId { get; set; }

    /// <summary>
    /// Идентификатор объекта недвижимости
    /// </summary>
    [Required]
    public Guid PropertyId { get; set; }

    /// <summary>
    /// Дата совершения сделки
    /// </summary>
    public DateTime DealDate { get; set; }

    /// <summary>
    /// Сумма сделки
    /// </summary>
    public decimal DealAmount { get; set; }

    /// <summary>
    /// Тип сделки
    /// </summary>
    public string DealType { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания записи о завершенной сделке
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления записи
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
