using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs
{
    /// <summary>
    /// DTO для представления продавца
    /// </summary>
    public class SellerDto
    {
        /// <summary>
        /// Идентификатор продавца
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор клиента, связанного с продавцом
        /// </summary>
        public Guid ClientId { get; set; }
        
        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegisteredAt { get; set; }
    }
}