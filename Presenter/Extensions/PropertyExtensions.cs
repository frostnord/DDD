using System;
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
            (
                    propertyEntity.Id.Value,
                    propertyEntity.Address?.ToString(),
                    propertyEntity.Price?.Value ?? 0,
                    propertyEntity.Status?.GetDisplayName(),
                    propertyEntity.Description?.Value,
                    propertyEntity.Details?.NumberOfRooms?.Value ?? 0,
                    propertyEntity.Details?.Floor?.Value ?? 0,
                    propertyEntity.Details?.TotalFloors?.Value ?? 0,
                    propertyEntity.Details?.Type?.Name,
                    propertyEntity.Details?.HeatingType?.Name,
                    propertyEntity.Details?.Condition?.Value,
                    propertyEntity.Details?.Area?.Value ?? 0,
                    propertyEntity.Details?.HasParking,
                    propertyEntity.GetCurrentOwner()?.OwnerClientId?.Value ?? Guid.Empty,
                    propertyEntity.CreatedAt,
                    propertyEntity.UpdatedAt
            );
        }
    }
}