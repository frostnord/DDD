using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.BuyerDTO
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
        public Guid ClientId { get; init; }

        /// <summary>
        /// Предпочтительное количество комнат
        /// </summary>
        public int PreferredNumberOfRooms { get; init; } 

        /// <summary>
        /// Предпочтительный этаж
        /// </summary>
        public int PreferredFloor { get; init; }

        /// <summary>
        /// Предпочтительное общее количество этажей в здании
        /// </summary>
        public int PreferredTotalFloors { get; init; } 

        /// <summary>
        /// Предпочтительный тип недвижимости
        /// </summary>
        public required string PreferredType { get; init; }

        /// <summary>
        /// Наличие предпочтения по парковке
        /// </summary>
        public bool? PreferParking { get; init; } 

        /// <summary>
        /// Предпочтительный тип отопления
        /// </summary>
        public required string PreferredHeatingType { get; init; } 

        /// <summary>
        /// Предпочтительное состояние недвижимости
        /// </summary>
        public required string PreferredCondition { get; init; }
    }
}