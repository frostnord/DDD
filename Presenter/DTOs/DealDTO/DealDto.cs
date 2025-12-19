using Domain.Deal;

namespace Presenter.DTOs.DealDTO
{
    /// <summary>
    /// DTO для представления информации о сделке
    /// </summary>
    public class DealDto
    {
        /// <summary>
        /// Уникальный идентификатор сделки
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Идентификатор клиента, участвующего в сделке
        /// </summary>
        public Guid ClientId { get; set; }
        
        /// <summary>
        /// Идентификатор объекта недвижимости, участвующего в сделке
        /// </summary>
        public Guid PropertyId { get; set; }
        
        /// <summary>
        /// Идентификатор бронирования, связанного со сделкой (опционально)
        /// </summary>
        public Guid? BookingId { get; set; }
        
        /// <summary>
        /// Детали сделки
        /// </summary>
        public DealDetails Details { get; set; }
        
        /// <summary>
        /// Текущий статус сделки
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Дата создания сделки
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Дата последнего обновления сделки
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}