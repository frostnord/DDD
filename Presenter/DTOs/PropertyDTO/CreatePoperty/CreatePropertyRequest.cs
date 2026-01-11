using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO.CreatePoperty
{
    /// <summary>
    /// DTO для запроса создания недвижимости
    /// </summary>
    public class CreatePropertyRequest
    {
        /// <summary>
        /// Адрес недвижимости
        /// </summary>
        [Required(ErrorMessage = "Адрес обязателен")]
        public required AddressDto Address { get; init; }

        /// <summary>
        /// Детали недвижимости
        /// </summary>
        [Required(ErrorMessage = "Детали недвижимости обязательны")]
        public required PropertyDetailsDto PropertyDetails { get; init; }

        /// <summary>
        /// Информация о владельце
        /// </summary>
        [Required(ErrorMessage = "Информация о владельце обязательна")]
        public required OwnershipDto Ownership { get; init; }
    }
}