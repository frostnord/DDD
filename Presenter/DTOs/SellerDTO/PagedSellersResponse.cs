using System.Collections.Generic;
using UseCases.DTO.Seller;

namespace Presenter.DTOs.SellerDTO
{
    public record PagedSellersResponse(
        IEnumerable<SellerDto> Items,
        int TotalCount,
        int PageSize,
        int TotalPages,
        int CurrentPage
    );
}