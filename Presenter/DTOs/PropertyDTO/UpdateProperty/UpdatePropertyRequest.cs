using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO.UpdateProperty
{
    /// <summary>
    /// DTO для запроса обновления данных о недвижимости
    /// </summary>
    public class UpdatePropertyRequest
    {
        /// <summary>
        /// Полный адрес объекта недвижимости.
        /// </summary>
        [Required]
        public required AddressDto Address { get; init; }

        /// <summary>
        /// Детальные характеристики объекта недвижимости.
        /// </summary>
        [Required]
        public required PropertyDetailsDto PropertyDetails { get; init; }

        /// <summary>
        /// Информация о текущем владельце объекта.
        /// </summary>
        [Required]
        public required OwnershipDto Ownership { get; init; }
    }
}