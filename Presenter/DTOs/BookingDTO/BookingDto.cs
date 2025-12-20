namespace Presenter.DTOs.BookingDTO
{
    /// <summary>
    /// DTO для бронирования
    /// </summary>
    public class BookingDto
    {
        public Guid Id { get; set; }
        
        public Guid ClientId { get; set; }
        
        public Guid PropertyId { get; set; }
        
        public Guid AgencyId { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public decimal TotalPrice { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}