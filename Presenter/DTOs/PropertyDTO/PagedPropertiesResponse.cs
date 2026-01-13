using UseCases.UseCases.DTO.Property;

namespace Presenter.DTOs.PropertyDTO
{
    public class PagedPropertiesResponse
    {
        public required IEnumerable<PropertyDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}