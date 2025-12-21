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
            var addressVO = Address.Create(command.Street, command.City, command.HomeNumber, command.ZipCode, command.Country);
            if (addressVO.IsFailure)
            {
                return Result.Failure<Guid>(addressVO.Error);
            }

            var priceVO = Price.Create(command.Price);
            if (priceVO.IsFailure)
            {
                return Result.Failure<Guid>(priceVO.Error);
            }

            var descriptionVO = Description.Create(command.Description);
            if (descriptionVO.IsFailure)
            {
                return Result.Failure<Guid>(descriptionVO.Error);
            }

            var propertyTypeVO = SmartPropertyType.FromName(command.PropertyType);
            var heatingTypeVO = HeatingType.Create(command.HeatingType);
            if (heatingTypeVO.IsFailure)
            {
                return Result.Failure<Guid>(heatingTypeVO.Error);
            }

            var propertyConditionVO = PropertyCondition.Create(command.PropertyCondition);
            if (propertyConditionVO.IsFailure)
            {
                return Result.Failure<Guid>(propertyConditionVO.Error);
            }

            var detailsVO = PropertyDetails.Create(
                command.Area,
                command.NumberOfRooms,
                command.Floor,
                command.TotalFloors,
                propertyTypeVO,
                false,
                command.HasParking ?? false,
                command.HeatingType,
                command.PropertyCondition);
            if (detailsVO.IsFailure)
            {
                return Result.Failure<Guid>(detailsVO.Error);
            }

            var ownerIdVO = ClientId.Create(command.OwnerClientId);
            if (ownerIdVO.IsFailure)
            {
                return Result.Failure<Guid>(ownerIdVO.Error);
            }

            var ownerRecordVO = OwnershipRecord.Create(ownerIdVO.Value, command.StartDate);
            if (ownerRecordVO.IsFailure)
            {
                return Result.Failure<Guid>(ownerRecordVO.Error);
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