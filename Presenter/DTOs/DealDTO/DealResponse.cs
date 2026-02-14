using System;
using Domain.Deal;
using Domain.Deal.VO;

namespace Presenter.DTOs.DealDTO
{
    /// <summary>
    /// DTO для представления информации о сделке
    /// </summary>
    public record DealResponse(
        Guid Id,
        Guid ClientId,
        Guid PropertyId,
        DealDetails Details,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}