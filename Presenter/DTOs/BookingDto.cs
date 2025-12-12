namespace Presenter.DTOs
{
    /// <summary>
    /// DTO для бронирования
    /// </summary>
    public class BookingDto
    {
        /// <summary>
        /// Идентификатор бронирования
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор клиента, совершающего бронирование
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Идентификатор объекта недвижимости, который бронируется
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// Идентификатор агентства, осуществляющего бронирование
        /// </summary>
        public Guid AgencyId { get; set; }

        /// <summary>
        /// Дата начала бронирования
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания бронирования
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Общая цена бронирования
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Дата создания бронирования
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления бронирования
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}