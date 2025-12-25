using System;

namespace Presenter.DTOs.SellerDTO
{
    /// <summary>
    /// DTO для представления продавца
    /// </summary>
    public record SellerDto(
        Guid Id,
        Guid ClientId,
        DateTime RegisteredAt
    );
}