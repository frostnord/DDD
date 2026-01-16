using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для адреса недвижимости.
    /// </summary>
    public record AddressDto
    {
        /// <summary>
        /// Название улицы.
        /// </summary>
        /// <example>ул. Ленина</example>
        [Required(ErrorMessage = "Улица обязательна")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название улицы должно содержать от 2 до 100 символов")]
        public required string Street { get; init; }

        /// <summary>
        /// Название города.
        /// </summary>
        /// <example>Москва</example>
        [Required(ErrorMessage = "Город обязателен")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название города должно содержать от 2 до 50 символов")]
        public required string City { get; init; }

        /// <summary>
        /// Номер дома.
        /// </summary>
        /// <example>10</example>
        [Required(ErrorMessage = "Номер дома обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Номер дома должен быть положительным числом")]
        public int HomeNumber { get; init; }

        /// <summary>
        /// Почтовый индекс.
        /// </summary>
        /// <example>123456</example>
        [Required(ErrorMessage = "Почтовый индекс обязателен")]
        [Range(100000, 999999, ErrorMessage = "Почтовый индекс должен быть 6-значным числом")]
        public int ZipCode { get; init; }

        /// <summary>
        /// Название страны.
        /// </summary>
        /// <example>Россия</example>
        [Required(ErrorMessage = "Страна обязательна")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название страны должно содержать от 2 до 50 символов")]
        public required string Country { get; init; }
    }
}