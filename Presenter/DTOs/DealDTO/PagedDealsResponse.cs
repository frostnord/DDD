using System.Collections.Generic;

namespace Presenter.DTOs.DealDTO
{
    public sealed record PagedDealsResponse(
        IEnumerable<DealResponse> Items,
        int TotalCount,
        int PageSize,
        int TotalPages,
        int CurrentPage
    );
}
