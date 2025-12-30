using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
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
        public AddressDto Address { get; init; }

        /// <summary>
        /// Детали недвижимости
        /// </summary>
        [Required(ErrorMessage = "Детали недвижимости обязательны")]
        public PropertyDetailsDto PropertyDetails { get; init; }

        /// <summary>
        /// Информация о владельце
        /// </summary>
        [Required(ErrorMessage = "Информация о владельце обязательна")]
        public OwnershipDto Ownership { get; init; }
    }
    
    public record AddressDto(
        string Street,
        string City,
        int HomeNumber,
        int ZipCode,
        string Country
    );
    
    public record PropertyDetailsDto(
        decimal Price,
        string Description,
        int NumberOfRooms,
        int Floor,
        int TotalFloors,
        decimal Area,
        string Type,
        string HeatingType,
        string Condition,
        bool? HasParking
    );
    
    public record OwnershipDto(
        Guid OwnerClientId,
        DateTime StartDate
    );
    
}