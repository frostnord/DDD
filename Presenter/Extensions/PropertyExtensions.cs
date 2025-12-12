using Domain.Property;
using Presenter.DTOs;
using Presenter.DTOs.PropertyDTO;

namespace Presenter.Extensions
{
    /// <summary>
    /// Расширения для преобразования Property в DTO и обратно
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// Преобразует Property в PropertyDto
        /// </summary>
        /// <param name="propertyEntity">Объект недвижимости</param>
        /// <returns>DTO недвижимости</returns>
        public static PropertyDto ToDTO(this PropertyEntity propertyEntity)
        {
            if (propertyEntity == null)
                return null;

            return new PropertyDto
            {
                Id = propertyEntity.Id.Value,
                Address = propertyEntity.Address?.ToString(),
                Price = propertyEntity.Price?.Value ?? 0,
                Status = propertyEntity.Status?.GetDisplayName(),
                Description = propertyEntity.Description?.Value,
                NumberOfRooms = propertyEntity.Details?.NumberOfRooms?.Value ?? 0,
                Floor = propertyEntity.Details?.Floor?.Value ?? 0,
                TotalFloors = propertyEntity.Details?.TotalFloors?.Value ?? 0,
                PropertyType = propertyEntity.Details?.Type?.Name,
                HeatingType = propertyEntity.Details?.HeatingType?.Name,
                PropertyCondition = propertyEntity.Details?.Condition?.Value,
                Area = propertyEntity.Details?.Area?.Value ?? 0,
                HasParking = propertyEntity.Details?.HasParking,
                OwnerClientId = propertyEntity.GetCurrentOwner()?.OwnerClientId?.Value ?? Guid.Empty,
                CreatedAt = propertyEntity.CreatedAt,
                UpdatedAt = propertyEntity.UpdatedAt
            };
        }
    }
}