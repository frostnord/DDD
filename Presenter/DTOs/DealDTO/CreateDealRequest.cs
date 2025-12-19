using System.ComponentModel.DataAnnotations;
using Domain.Deal;

namespace Presenter.DTOs.DealDTO
{
    /// <summary>
    /// Запрос на создание сделки
    /// </summary>
    public class CreateDealRequest
    {
        /// <summary>
        /// Идентификатор клиента, участвующего в сделке
        /// </summary>
        [Required(ErrorMessage = "Идентификатор клиента обязателен")]
        public Guid ClientId { get; set; }

        /// <summary>
        /// Идентификатор объекта недвижимости, участвующего в сделке
        /// </summary>
        [Required(ErrorMessage = "Идентификатор объекта недвижимости обязателен")]
        public Guid PropertyId { get; set; }

        /// <summary>
        /// Идентификатор бронирования, связанного со сделкой (опционально)
        /// </summary>
        public Guid? BookingId { get; set; }

        /// <summary>
        /// Детали сделки
        /// </summary>
        [Required(ErrorMessage = "Детали сделки обязательны")]
        public DealDetails Details { get; set; }
    }
}