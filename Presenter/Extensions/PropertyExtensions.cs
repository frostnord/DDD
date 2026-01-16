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
        public static PropertyDto? ToDTO(this PropertyEntity propertyEntity)
        {
            if (propertyEntity == null)
                return null;

            var currentOwner = propertyEntity.GetCurrentOwner();

            return new PropertyDto(
                propertyEntity.Id.Value,
                new AddressDto(
                    propertyEntity.Address.Street,
                    propertyEntity.Address.City,
                    propertyEntity.Address.HomeNumber,
                    propertyEntity.Address.ZipCode,
                    propertyEntity.Address.Country
                ),
                new PropertyDetailsDto(
                    propertyEntity.Price.Value,
                    propertyEntity.Description.Value,
                    propertyEntity.PropertyDetails.NumberOfRooms.Value,
                    propertyEntity.PropertyDetails.Floor.Value,
                    propertyEntity.PropertyDetails.TotalFloors.Value,
                    propertyEntity.PropertyDetails.Area.Value,
                    propertyEntity.PropertyDetails.Type.Name,
                    propertyEntity.PropertyDetails.HeatingType.Value.ToString(),
                    propertyEntity.PropertyDetails.Condition.Value,
                    propertyEntity.PropertyDetails.HasParking
                ),
                new OwnershipDto(
                    currentOwner?.OwnerClientId?.Value ?? Guid.Empty,
                    currentOwner?.StartDate ?? DateTime.MinValue
                )
            );
        }
    }
}
