using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.CompletedDealDTO;

/// <summary>
/// Запрос на создание завершенной сделки
/// </summary>
public class CreateCompletedDealRequest
{
    /// <summary>
    /// Идентификатор клиента-покупателя (обязательное поле)
    /// Должен быть действительным GUID
    /// </summary>
    [Required(ErrorMessage = "Идентификатор клиента-покупателя обязателен")]
    public Guid BuyerClientId { get; init; }

    /// <summary>
    /// Идентификатор клиента-продавца (обязательное поле)
    /// Должен быть действительным GUID
    /// </summary>
    [Required(ErrorMessage = "Идентификатор клиента-продавца обязателен")]
    public Guid SellerClientId { get; init; }

    /// <summary>
    /// Идентификатор объекта недвижимости (обязательное поле)
    /// Должен быть действительным GUID
    /// </summary>
    [Required(ErrorMessage = "Идентификатор объекта недвижимости обязателен")]
    public Guid PropertyId { get; init; }

    /// <summary>
    /// Дата совершения сделки (обязательное поле)
    /// Должна быть действительной датой, не позже текущей даты
    /// </summary>
    [Required(ErrorMessage = "Дата сделки обязательна")]
    public DateTime DealDate { get; init; }

    /// <summary>
    /// Сумма сделки (обязательное поле)
    /// Должна быть положительной
    /// </summary>
    [Required(ErrorMessage = "Сумма сделки обязательна")]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Сумма сделки должна быть положительной")]
    public decimal DealAmount { get; init; }

    /// <summary>
    /// Тип сделки (обязательное поле)
    /// Должно соответствовать одному из допустимых типов сделок
    /// Максимальная длина определяется константой MAX_DEAL_TYPE_LENGTH
    /// </summary>
    [Required(ErrorMessage = "Тип сделки обязателен")]
    [MaxLength(Domain.Deal.DealType.MAX_DEAL_TYPE_LENGTH, ErrorMessage = "Превышена максимальная длина типа сделки")]
    [RegularExpression(@"^(Purchase|Rent|Lease)$", ErrorMessage = "Недопустимый тип сделки. Разрешены: Purchase, Rent, Lease")]
    public required string DealType { get; init; }
}