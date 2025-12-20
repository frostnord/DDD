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

        /// <summary>
        /// Идентификатор агентства, осуществляющего бронирование
        /// </summary>
        [Required(ErrorMessage = "Идентификатор агентства обязателен")]
        public Guid AgencyId { get; init; }

        /// <summary>
        /// Дата начала бронирования
        /// </summary>
        [Required(ErrorMessage = "Дата начала бронирования обязательна")]
        public DateTime StartDate { get; init; }

        /// <summary>
        /// Дата окончания бронирования
        /// </summary>
        [Required(ErrorMessage = "Дата окончания бронирования обязательна")]
        public DateTime EndDate { get; init; }

        /// <summary>
        /// Общая цена бронирования
        /// </summary>
        [Required(ErrorMessage = "Общая цена обязательна")]
        [Range(1, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal TotalPrice { get; init; }
    }
}