using System;
using System.ComponentModel.DataAnnotations;
using Domain.Deal;

namespace Presenter.DTOs.DealDTO
{
    /// <summary>
    /// Запрос на создание сделки
    /// </summary>
    public record CreateDealRequest
    {
        /// <summary>
        /// Идентификатор клиента, участвующего в сделке
        /// </summary>
        [Required(ErrorMessage = "Идентификатор клиента обязателен")]
        public Guid ClientId { get; init; }

        /// <summary>
        /// Идентификатор объекта недвижимости, участвующего в сделке
        /// </summary>
        [Required(ErrorMessage = "Идентификатор объекта недвижимости обязателен")]
        public Guid PropertyId { get; init; }

        /// <summary>
        /// Идентификатор бронирования, связанного со сделкой (опционально)
        /// </summary>
        public Guid? BookingId { get; init; }

        /// <summary>
        /// Детали сделки
        /// </summary>
        [Required(ErrorMessage = "Детали сделки обязательны")]
        public required DealDetails Details { get; init; }
    }
}