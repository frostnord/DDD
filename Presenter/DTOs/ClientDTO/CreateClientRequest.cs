using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.ClientDTO
{
    /// <summary>
    /// DTO для запроса создания клиента
    /// </summary>
    public class CreateClientRequest
    {
        /// <summary>
        /// Имя клиента
        /// </summary>
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 200 символов")]
        public required string FirstName { get; init; }

        /// <summary>
        /// Фамилия клиента
        /// </summary>
        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 200 символов")]
        public required string LastName { get; init; }

        /// <summary>
        /// Email клиента
        /// </summary>
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [StringLength(100, ErrorMessage = "Email не может превышать 100 символов")]
        public required string Email { get; init; }

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        [Required(ErrorMessage = "Номер телефона обязателен")]
        [RegularExpression(@"^(\+7|8)(\s|-|\()?([0-9]{3})(\s|-|\))?([0-9]{3})(\s|-)?([0-9]{2})(\s|-)?([0-9]{2})$",
            ErrorMessage = "Некорректный формат российского номера телефона")]
        public required string PhoneNumber { get; init; }
    }
}