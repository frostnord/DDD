using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs
{
    /// <summary>
    /// Запрос на обновление статуса сделки
    /// </summary>
    public class UpdateDealStatusRequest
    {
        /// <summary>
        /// Новый статус сделки
        /// </summary>
        [Required(ErrorMessage = "Статус сделки обязателен")]
        [RegularExpression("^(Confirmed|Completed|Cancelled)$", ErrorMessage = "Недопустимый статус сделки")]
        public string Status { get; set; }
    }
}