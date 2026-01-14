using System.Collections.Generic;
using UseCases.DTO.Seller;

namespace Presenter.DTOs.SellerDTO
{
    public class PagedSellersResponse
    {
        public IEnumerable<SellerDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}