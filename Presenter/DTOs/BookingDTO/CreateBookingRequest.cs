using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.BookingDTO
{
    /// <summary>
    /// DTO для запроса создания бронирования
    /// </summary>
    public class CreateBookingRequest
    {
        /// <summary>
        /// Идентификатор клиента, совершающего бронирование
        /// </summary>
        [Required(ErrorMessage = "Идентификатор клиента обязателен")]
        public Guid ClientId { get; init; }

        /// <summary>
        /// Идентификатор объекта недвижимости, который бронируется
        /// </summary>
        [Required(ErrorMessage = "Идентификатор объекта недвижимости обязателен")]
        public Guid PropertyId { get; init; }
    }
}