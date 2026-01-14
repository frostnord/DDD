using UseCases.UseCases.DTO.Buyer;

namespace Presenter.DTOs.BuyerDTO
{
    public class PagedBuyersResponse
    {
        public IReadOnlyList<BuyerDto> Items { get; set; } = new List<BuyerDto>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}