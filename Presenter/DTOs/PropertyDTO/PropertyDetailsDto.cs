using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для детальных характеристик недвижимости.
    /// </summary>
    public record PropertyDetailsDto
    {
        /// <summary>
        /// Цена объекта недвижимости.
        /// </summary>
        /// <example>5000000.00</example>
        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть положительным числом")]
        public decimal Price { get; init; }

        /// <summary>
        /// Текстовое описание объекта.
        /// </summary>
        /// <example>Просторная квартира в центре города с отличным видом.</example>
        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Описание должно содержать от 10 до 1000 символов")]
        public required string Description { get; init; }

        /// <summary>
        /// Количество комнат.
        /// </summary>
        /// <example>3</example>
        [Required(ErrorMessage = "Количество комнат обязательно")]
        [Range(1, 100, ErrorMessage = "Количество комнат должно быть от 1 до 100")]
        public int NumberOfRooms { get; init; }

        /// <summary>
        /// Этаж, на котором находится объект.
        /// </summary>
        /// <example>5</example>
        [Required(ErrorMessage = "Этаж обязателен")]
        [Range(1, 200, ErrorMessage = "Этаж должен быть от 1 до 200")]
        public int Floor { get; init; }

        /// <summary>
        /// Общее количество этажей в здании.
        /// </summary>
        /// <example>12</example>
        [Required(ErrorMessage = "Общее количество этажей обязательно")]
        [Range(1, 200, ErrorMessage = "Общее количество этажей должно быть от 1 до 200")]
        public int TotalFloors { get; init; }

        /// <summary>
        /// Площадь объекта в квадратных метрах.
        /// </summary>
        /// <example>75.5</example>
        [Required(ErrorMessage = "Площадь обязательна")]
        [Range(1, 10000, ErrorMessage = "Площадь должна быть от 1 до 10000 кв.м.")]
        public decimal Area { get; init; }

        /// <summary>
        /// Тип недвижимости (например, "Квартира", "Дом").
        /// </summary>
        /// <example>Квартира</example>
        [Required(ErrorMessage = "Тип недвижимости обязателен")]
        public required string Type { get; init; }

        /// <summary>
        /// Тип отопления (например, "Центральное", "Индивидуальное").
        /// </summary>
        /// <example>Центральное</example>
        [Required(ErrorMessage = "Тип отопления обязателен")]
        public required string HeatingType { get; init; }

        /// <summary>
        /// Состояние объекта (например, "Новый", "Требует ремонта").
        /// </summary>
        /// <example>Новый</example>
        [Required(ErrorMessage = "Состояние недвижимости обязательно")]
        public required string Condition { get; init; }

        /// <summary>
        /// Наличие парковки.
        /// </summary>
        /// <example>true</example>
        public bool? HasParking { get; init; }
    }
}