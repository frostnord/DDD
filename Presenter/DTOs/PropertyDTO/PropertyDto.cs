namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для недвижимости
    /// </summary>
    public class PropertyDto
    {
        /// <summary>
        /// Идентификатор недвижимости
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Адрес недвижимости
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Цена недвижимости
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Статус недвижимости
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Описание недвижимости
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Количество комнат
        /// </summary>
        public int NumberOfRooms { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// Общее количество этажей в здании
        /// </summary>
        public int TotalFloors { get; set; }

        /// <summary>
        /// Тип недвижимости
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// Тип отопления
        /// </summary>
        public string HeatingType { get; set; }

        /// <summary>
        /// Состояние недвижимости
        /// </summary>
        public string PropertyCondition { get; set; }

        /// <summary>
        /// Площадь недвижимости
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Наличие парковки
        /// </summary>
        public bool? HasParking { get; set; }

        /// <summary>
        /// Идентификатор клиента-владельца
        /// </summary>
        public Guid OwnerClientId { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}