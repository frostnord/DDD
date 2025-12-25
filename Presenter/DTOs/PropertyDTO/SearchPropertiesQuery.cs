namespace Presenter.DTOs.PropertyDTO
{
    public class SearchPropertiesQuery
    {
        public string? City { get; set; }
        public string? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public int? MinRooms { get; set; }
        public int? MaxRooms { get; set; }
        public int? MinFloor { get; set; }
        public int? MaxFloor { get; set; }
        public string? HeatingType { get; set; }
        public string? PropertyCondition { get; set; }
        public bool? HasParking { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}