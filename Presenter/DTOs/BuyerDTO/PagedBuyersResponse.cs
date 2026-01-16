using System.Collections.Generic;
using UseCases.UseCases.DTO.Buyer;

namespace Presenter.DTOs.BuyerDTO
{
    public record PagedBuyersResponse(
        IEnumerable<BuyerDto> Items,
        int TotalCount,
        int PageSize,
        int TotalPages,
        int CurrentPage
    );
}