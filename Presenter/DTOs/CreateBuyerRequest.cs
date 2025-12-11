using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs
{
    /// <summary>
    /// DTO для запроса создания покупателя
    /// </summary>
    public class CreateBuyerRequest
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        [Required(ErrorMessage = "Идентификатор клиента обязателен")]
        public Guid ClientId { get; set; }

        /// <summary>
        /// Предпочтительное количество комнат
        /// </summary>
        public int PreferredNumberOfRooms { get; set; } = 2;

        /// <summary>
        /// Предпочтительный этаж
        /// </summary>
        public int PreferredFloor { get; set; } = 3;

        /// <summary>
        /// Предпочтительное общее количество этажей в здании
        /// </summary>
        public int PreferredTotalFloors { get; set; } = 9;

        /// <summary>
        /// Предпочтительный тип недвижимости
        /// </summary>
        public string PreferredType { get; set; } = "Apartment";

        /// <summary>
        /// Наличие предпочтения по парковке
        /// </summary>
        public bool? PreferParking { get; set; } = true;

        /// <summary>
        /// Предпочтительный тип отопления
        /// </summary>
        public string PreferredHeatingType { get; set; } = "Central";

        /// <summary>
        /// Предпочтительное состояние недвижимости
        /// </summary>
        public string PreferredCondition { get; set; } = "Хорошее";
    }
}