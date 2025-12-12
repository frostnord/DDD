using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace UseCases.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Result<PropertyEntity>> CreatePropertyAsync(string street, string city, int homeNumber, int zipCode,
            string country, decimal price, string description, int numberOfRooms, int floor, int totalFloors,
            string propertyType, string heatingType, string propertyCondition, double area, bool? hasParking,
            Guid ownerClientId, DateTime startDate)
        {
            var addressVO = Address.Create(street, city, homeNumber, zipCode, country);
            if (addressVO.IsFailure)
                return Result.Failure<PropertyEntity>(addressVO.Error);

            var priceVO = Price.Create(price);
            if (priceVO.IsFailure)
                return Result.Failure<PropertyEntity>(priceVO.Error);

            var descriptionVO = Description.Create(description);
            if (descriptionVO.IsFailure)
                return Result.Failure<PropertyEntity>(descriptionVO.Error);

            var propertyTypeVO = SmartPropertyType.FromName(propertyType);
            var heatingTypeVO = HeatingType.Create(heatingType);
            if (heatingTypeVO.IsFailure)
                return Result.Failure<PropertyEntity>(heatingTypeVO.Error);

            var propertyConditionVO = PropertyCondition.Create(propertyCondition);
            if (propertyConditionVO.IsFailure)
                return Result.Failure<PropertyEntity>(propertyConditionVO.Error);

            var detailsVO = PropertyDetails.Create(
                (int)area, numberOfRooms, floor, totalFloors,
                propertyTypeVO, false, hasParking ?? false, heatingType, propertyCondition);
            if (detailsVO.IsFailure)
                return Result.Failure<PropertyEntity>(detailsVO.Error);

            var ownerIdVO = ClientId.Create(ownerClientId);
            if (ownerIdVO.IsFailure)
                return Result.Failure<PropertyEntity>(ownerIdVO.Error);

            var ownerRecordVO = OwnershipRecord.Create(ownerIdVO.Value, startDate);
            if (ownerRecordVO.IsFailure)
                return Result.Failure<PropertyEntity>(ownerRecordVO.Error);

            var propertyResult = PropertyEntity.Create(
                addressVO.Value,
                priceVO.Value,
                descriptionVO.Value,
                detailsVO.Value
            );

            if (propertyResult.IsFailure)
                return Result.Failure<PropertyEntity>(propertyResult.Error);

            var property = propertyResult.Value;
            property.SetFirstOwner(ownerRecordVO.Value);

            var saveResult = await _propertyRepository.AddAsync(property);
            if (saveResult.IsFailure)
                return Result.Failure<PropertyEntity>(saveResult.Error);

            return Result.Success(property);
        }

        public async Task<Result<PropertyEntity>> GetPropertyByIdAsync(Guid propertyId)
        {
            var propertyIdVO = PropertyId.Create(propertyId);
            if (propertyIdVO.IsFailure)
                return Result.Failure<PropertyEntity>(propertyIdVO.Error);

            return await _propertyRepository.GetByIdAsync(propertyIdVO.Value);
        }

        public async Task<Result<IEnumerable<PropertyEntity>>> GetAllPropertiesAsync()
        {
            return await _propertyRepository.GetAllAsync();
        }

        public async Task<Result> UpdatePropertyAsync(Guid propertyId, string street, string city, int homeNumber,
            int zipCode, string country, decimal price, string description, int numberOfRooms, int floor,
            int totalFloors, string propertyType, string heatingType, string propertyCondition, double area,
            bool? hasParking)
        {
            var getPropertyResult = await GetPropertyByIdAsync(propertyId);
            if (getPropertyResult.IsFailure)
                return Result.Failure(getPropertyResult.Error);

            var property = getPropertyResult.Value;

            var addressVO = Address.Create(street, city, homeNumber, zipCode, country);
            if (addressVO.IsFailure)
                return Result.Failure(addressVO.Error);

            var priceVO = Price.Create(price);
            if (priceVO.IsFailure)
                return Result.Failure(priceVO.Error);

            var descriptionVO = Description.Create(description);
            if (descriptionVO.IsFailure)
                return Result.Failure(descriptionVO.Error);

            var propertyTypeVO = SmartPropertyType.FromName(propertyType);
            var heatingTypeVO = HeatingType.Create(heatingType);
            if (heatingTypeVO.IsFailure)
                return Result.Failure(heatingTypeVO.Error);

            var propertyConditionVO = PropertyCondition.Create(propertyCondition);
            if (propertyConditionVO.IsFailure)
                return Result.Failure(propertyConditionVO.Error);

            var detailsVO = PropertyDetails.Create(
                (int)area, numberOfRooms, floor, totalFloors,
                propertyTypeVO, false, hasParking ?? false, heatingType, propertyCondition);
            if (detailsVO.IsFailure)
                return Result.Failure(detailsVO.Error);

            // Обновляем свойства недвижимости через рефлексию, так как в доменной модели нет методов обновления
            typeof(PropertyEntity).GetProperty("Address")?.SetValue(property, addressVO.Value);
            property.UpdatePrice(priceVO.Value);
            property.UpdateDescription(descriptionVO.Value);
            typeof(PropertyEntity).GetProperty("Details")?.SetValue(property, detailsVO.Value);

            return await _propertyRepository.UpdateAsync(property);
        }

        public async Task<Result<IEnumerable<PropertyEntity>>> SearchPropertiesAsync(string? city, string? propertyType,
            decimal? minPrice, decimal? maxPrice, int? minArea, int? maxArea, int? minRooms, int? maxRooms,
            int? minFloor, int? maxFloor, string? heatingType, string? propertyCondition, bool? hasParking)
        {
            var properties = await _propertyRepository.GetAllAsync();
            if (properties.IsFailure)
                return Result.Failure<IEnumerable<PropertyEntity>>(properties.Error);

            var filteredProperties = properties.Value.AsEnumerable();

            if (!string.IsNullOrEmpty(city))
                filteredProperties =
                    filteredProperties.Where(p => p.Address.City.Equals(city, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(propertyType))
                filteredProperties = filteredProperties.Where(p =>
                    p.Details.Type.Name.Equals(propertyType, StringComparison.OrdinalIgnoreCase));

            if (minPrice.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Price.Value >= minPrice.Value);

            if (maxPrice.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Price.Value <= maxPrice.Value);

            if (minArea.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.Area.Value >= minArea.Value);

            if (maxArea.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.Area.Value <= maxArea.Value);

            if (minRooms.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.NumberOfRooms.Value >= minRooms.Value);

            if (maxRooms.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.NumberOfRooms.Value <= maxRooms.Value);

            if (minFloor.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.Floor.Value >= minFloor.Value);

            if (maxFloor.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.Floor.Value <= maxFloor.Value);

            if (!string.IsNullOrEmpty(heatingType))
                filteredProperties = filteredProperties.Where(p =>
                    p.Details.HeatingType.Name.Equals(heatingType, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(propertyCondition))
                filteredProperties = filteredProperties.Where(p =>
                    p.Details.Condition.Value.Equals(propertyCondition, StringComparison.OrdinalIgnoreCase));

            if (hasParking.HasValue)
                filteredProperties = filteredProperties.Where(p => p.Details.HasParking == hasParking.Value);

            return Result.Success(filteredProperties);
        }

        public async Task<Result> DeletePropertyAsync(Guid propertyId)
        {
            var propertyIdVO = PropertyId.Create(propertyId);
            if (propertyIdVO.IsFailure)
                return Result.Failure(propertyIdVO.Error);

            var property = await _propertyRepository.GetByIdAsync(propertyIdVO.Value);
            if (property.IsFailure)
                return Result.Failure(property.Error);

            return await _propertyRepository.DeleteAsync(propertyIdVO.Value);
        }
    }
}