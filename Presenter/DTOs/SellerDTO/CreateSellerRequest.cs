using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.SellerDTO
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
        public Guid ClientId { get; init; }
    }
}