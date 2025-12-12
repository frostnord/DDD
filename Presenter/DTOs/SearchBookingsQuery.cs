namespace Presenter.DTOs
{
    public class SearchBookingsQuery
    {
        public Guid? ClientId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? AgencyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}