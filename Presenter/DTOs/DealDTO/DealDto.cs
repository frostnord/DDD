using Domain.Deal;

namespace Presenter.DTOs.DealDTO
{
    /// <summary>
    /// DTO для представления информации о сделке
    /// </summary>
    public class DealDto
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public Guid PropertyId { get; set; }

        public Guid? BookingId { get; set; }

        public DealDetails Details { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}