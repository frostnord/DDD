using System;

namespace Presenter.DTOs.DealDTO
{
    /// <summary>
    /// Параметры поиска и фильтрации сделок
    /// </summary>
    public sealed record SearchDealsQuery(
        Guid? ClientId,
        Guid? PropertyId
    );
}