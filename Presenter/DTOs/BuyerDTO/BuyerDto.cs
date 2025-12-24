using System;

namespace Presenter.DTOs.BuyerDTO
{
    /// <summary>
    /// DTO для представления покупателя
    /// </summary>
    public record BuyerDto(Guid Id, Guid ClientId, DateTime RegisteredAt);
}