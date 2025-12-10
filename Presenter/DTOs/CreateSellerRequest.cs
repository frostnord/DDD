using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs
{
    /// <summary>
    /// DTO для запроса создания продавца
    /// </summary>
    public class CreateSellerRequest
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        [Required(ErrorMessage = "Идентификатор клиента обязателен")]
        public Guid ClientId { get; set; }
    }
}
