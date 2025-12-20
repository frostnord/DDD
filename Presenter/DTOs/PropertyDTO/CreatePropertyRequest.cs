using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для запроса создания недвижимости
    /// </summary>
    public class CreatePropertyRequest
    {
        /// <summary>
        /// Улица
        /// </summary>
        [Required(ErrorMessage = "Улица обязательна")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Улица должна содержать от 2 до 200 символов")]
        public string Street { get; init; }

        /// <summary>
        /// Город
        /// </summary>
        [Required(ErrorMessage = "Город обязателен")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Город должен содержать от 2 до 100 символов")]
        public string City { get; init; }

        /// <summary>
        /// Номер дома
        /// </summary>
        [Required(ErrorMessage = "Номер дома обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Номер дома должен быть положительным числом")]
        public int HomeNumber { get; init; }

        /// <summary>
        /// Почтовый индекс
        /// </summary>
        [Required(ErrorMessage = "Почтовый индекс обязателен")]
        [Range(100000, 999999, ErrorMessage = "Почтовый индекс должен быть 6-значным числом")]
        public int ZipCode { get; init; }

        /// <summary>
        /// Страна
        /// </summary>
        [Required(ErrorMessage = "Страна обязательна")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название страны должно содержать от 2 до 50 символов")]
        public string Country { get; init; }

        /// <summary>
        /// Цена недвижимости
        /// </summary>
        [Required(ErrorMessage = "Цена обязательна")]
        [Range(1, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; init; }

        /// <summary>
        /// Описание недвижимости
        /// </summary>
        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Описание должно содержать от 10 до 1000 символов")]
        public string Description { get; init; }

        /// <summary>
        /// Количество комнат
        /// </summary>
        [Required(ErrorMessage = "Количество комнат обязательно")]
        [Range(1, 100, ErrorMessage = "Количество комнат должно быть от 1 до 100")]
        public int NumberOfRooms { get; init; }

        /// <summary>
        /// Этаж
        /// </summary>
        [Required(ErrorMessage = "Этаж обязателен")]
        [Range(1, 100, ErrorMessage = "Этаж должен быть от 1 до 100")]
        public int Floor { get; init; }

        /// <summary>
        /// Общее количество этажей в здании
        /// </summary>
        [Required(ErrorMessage = "Общее количество этажей обязательно")]
        [Range(1, 100, ErrorMessage = "Общее количество этажей должно быть от 1 до 100")]
        public int TotalFloors { get; init; }

        /// <summary>
        /// Тип недвижимости
        /// </summary>
        [Required(ErrorMessage = "Тип недвижимости обязателен")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Тип недвижимости должен содержать от 2 до 50 символов")]
        public string PropertyType { get; init; }

        /// <summary>
        /// Тип отопления
        /// </summary>
        [Required(ErrorMessage = "Тип отопления обязателен")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Тип отопления должен содержать от 2 до 50 символов")]
        public string HeatingType { get; init; }

        /// <summary>
        /// Состояние недвижимости
        /// </summary>
        [Required(ErrorMessage = "Состояние недвижимости обязательно")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Состояние недвижимости должно содержать от 2 до 50 символов")]
        public string PropertyCondition { get; init; }

        /// <summary>
        /// Площадь недвижимости
        /// </summary>
        [Required(ErrorMessage = "Площадь обязательна")]
        [Range(1, double.MaxValue, ErrorMessage = "Площадь должна быть больше 0")]
        public double Area { get; init; }

        /// <summary>
        /// Наличие парковки
        /// </summary>
        public bool? HasParking { get; init; }

        /// <summary>
        /// Идентификатор клиента-владельца
        /// </summary>
        [Required(ErrorMessage = "Идентификатор владельца обязателен")]
        public Guid OwnerClientId { get; init; }

        /// <summary>
        /// Дата начала владения
        /// </summary>
        public DateTime StartDate { get; init; } 
    }
}