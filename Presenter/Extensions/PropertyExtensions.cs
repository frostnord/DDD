using System;
using Domain.Property;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.UseCases.DTO.Property;

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

            return new PropertyDto(
                propertyEntity.Id.Value,
                new AddressDto(
                    propertyEntity.Address?.Street ?? string.Empty,
                    propertyEntity.Address?.City ?? string.Empty,
                    propertyEntity.Address?.HomeNumber ?? 0,
                    propertyEntity.Address?.ZipCode ?? 0,
                    propertyEntity.Address?.Country ?? string.Empty
                ),
                new PropertyDetailsDto(
                    propertyEntity.Price?.Value ?? 0,
                    propertyEntity.Description?.Value ?? string.Empty,
                    propertyEntity.PropertyDetails?.NumberOfRooms?.Value ?? 0,
                    propertyEntity.PropertyDetails?.Floor?.Value ?? 0,
                    propertyEntity.PropertyDetails?.TotalFloors?.Value ?? 0,
                    propertyEntity.PropertyDetails?.Area?.Value ?? 0,
                    propertyEntity.PropertyDetails?.Type?.Name ?? string.Empty,
                    propertyEntity.PropertyDetails?.HeatingType?.Value.ToString() ?? string.Empty,
                    propertyEntity.PropertyDetails?.Condition?.Value ?? string.Empty,
                    propertyEntity.PropertyDetails?.HasParking
                ),
                new OwnershipDto(
                    propertyEntity.GetCurrentOwner()?.OwnerClientId?.Value ?? Guid.Empty,
                    propertyEntity.GetCurrentOwner()?.StartDate ?? DateTime.MinValue
                )
            );
        }
    }
}
