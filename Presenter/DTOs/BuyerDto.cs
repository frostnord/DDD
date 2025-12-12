namespace Presenter.DTOs
{
    /// <summary>
    /// DTO для представления покупателя
    /// </summary>
    public class BuyerDto
    {
        /// <summary>
        /// Идентификатор покупателя
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор клиента, связанного с покупателем
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegisteredAt { get; set; }
    }
}