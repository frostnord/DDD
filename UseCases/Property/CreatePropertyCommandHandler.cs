using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Property
{
    public class CreatePropertyCommandHandler : ICommandHandler<CreatePropertyCommand, Guid>
    {
        private readonly IPropertyRepository _propertyRepository;

        public CreatePropertyCommandHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Result<Guid>> HandleAsync(CreatePropertyCommand command)
        {
            
            var (Street, City, HomeNumber, ZipCode, Country) = command.AddressData;
            
            var (PropertyPrice, PropertyDescription, NumberOfRooms, Floor, TotalFloors,
                Area, PropertyType, Heating, Condition, HasParking) = command.PropertyDetailsData;
            
            var propertyTypeVO = SmartPropertyType.FromName(PropertyType);
            
            var (OwnerClientId, StartDate) = command.OwnershipData;

            var ownerIdVO = ClientId.Create(OwnerClientId);
            if (ownerIdVO.IsFailure)
            {
                return Result.Failure<Guid>(ownerIdVO.Error);
            }

            var ownerRecordVO = OwnershipRecord.Create(ownerIdVO.Value, StartDate);
            if (ownerRecordVO.IsFailure)
            {
                return Result.Failure<Guid>(ownerRecordVO.Error);
            }
            
            //addressVO
            var addressVO = Address.Create(Street, City, HomeNumber, ZipCode, Country);
            if (addressVO.IsFailure)
            {
                return Result.Failure<Guid>(addressVO.Error);
            }
            
            //priceVO
            var priceVO = Price.Create(PropertyPrice);
            if (priceVO.IsFailure)
            {
                return Result.Failure<Guid>(priceVO.Error);
            }
            
            //descriptionVO
            var descriptionVO = Description.Create(PropertyDescription);
            if (descriptionVO.IsFailure)
            {
                return Result.Failure<Guid>(descriptionVO.Error);
            }

            //detailsVO
            var detailsVO = PropertyDetails.Create(
                Area,
                NumberOfRooms,
                Floor,
                TotalFloors,
                propertyTypeVO,
                false,
                HasParking ?? false,
                Heating,
                Condition);
            if (detailsVO.IsFailure)
            {
                return Result.Failure<Guid>(detailsVO.Error);
            }
            
            var propertyResult = PropertyEntity.Create(
                addressVO.Value,
                priceVO.Value,
                descriptionVO.Value,
                detailsVO.Value
            );

            if (propertyResult.IsFailure)
            {
                return Result.Failure<Guid>(propertyResult.Error);
            }

            var property = propertyResult.Value;
            property.SetFirstOwner(ownerRecordVO.Value);

            var saveResult = await _propertyRepository.AddAsync(property);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Guid>(saveResult.Error);
            }

            return Result.Success(property.Id.Value);
        }
    }
}