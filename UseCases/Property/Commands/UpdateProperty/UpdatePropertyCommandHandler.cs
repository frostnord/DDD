using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Property.Commands.UpdateProperty;

public class UpdatePropertyCommandHandler : ICommandHandler<UpdatePropertyCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdatePropertyCommand command)
    {
        var propertyIdVO = PropertyId.Create(command.PropertyId);
        if (propertyIdVO.IsFailure)
            return Result.Failure(propertyIdVO.Error);

        var propertyResult = await _unitOfWork.Properties.GetByIdAsync(propertyIdVO.Value);
        if (propertyResult.IsFailure)
        {
            return Result.Failure(propertyResult.Error);
        }
        var property = propertyResult.Value;

        var (Street, City, HomeNumber, ZipCode, Country) = command.AddressDto;
        var (PropertyPrice, PropertyDescription, NumberOfRooms, Floor, TotalFloors, Area, Type, Heating, Condition, HasParking) = command.PropertyDetailsDto;
        var (OwnerClientId, StartDate) = command.OwnershipDto;

        var addressVO = Address.Create(Street, City, HomeNumber, ZipCode, Country);
        if (addressVO.IsFailure)
            return Result.Failure(addressVO.Error);

        var priceVO = Price.Create(PropertyPrice);
        if (priceVO.IsFailure)
            return Result.Failure(priceVO.Error);

        var descriptionVO = Description.Create(PropertyDescription);
        if (descriptionVO.IsFailure)
            return Result.Failure(descriptionVO.Error);

        var propertyTypeVO = SmartPropertyType.FromName(Type);
        var detailsVO = PropertyDetails.Create(
            Area,
            NumberOfRooms,
            Floor,
            TotalFloors,
            propertyTypeVO,
            false,
            HasParking ?? false,
            Heating,
            Condition
        );
        if (detailsVO.IsFailure)
            return Result.Failure(detailsVO.Error);

        var ownerIdVO = ClientId.Create(OwnerClientId);
        if (ownerIdVO.IsFailure)
            return Result.Failure(ownerIdVO.Error);

        var ownershipRecordVO = OwnershipRecord.Create(ownerIdVO.Value, StartDate);
        if (ownershipRecordVO.IsFailure)
            return Result.Failure(ownershipRecordVO.Error);

        // Обновление свойств объекта недвижимости
        property.UpdateAddress(addressVO.Value);
        property.UpdatePrice(priceVO.Value);
        property.UpdateDescription(descriptionVO.Value);
        property.UpdateDetails(detailsVO.Value);
        property.UpdateOwner(ownershipRecordVO.Value);

        var updateResult = _unitOfWork.Properties.Update(property);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
