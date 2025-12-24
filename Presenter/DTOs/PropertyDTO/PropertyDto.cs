using System;

namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для недвижимости
    /// </summary>
    public record PropertyDto(
        Guid Id,
        string Address,
        decimal Price,
        string Status,
        string Description,
        int NumberOfRooms,
        int Floor,
        int TotalFloors,
        string PropertyType,
        string HeatingType,
        string PropertyCondition,
        decimal Area,
        bool? HasParking,
        Guid OwnerClientId,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}