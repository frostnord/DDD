using CSharpFunctionalExtensions;
using Domain.Property;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Property
{
    public class CreatePropertyCommandHandler : ICommandHandler<CreatePropertyCommand, PropertyEntity>
    {
        private readonly IPropertyRepository _propertyRepository;

        public CreatePropertyCommandHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Result<PropertyEntity>> HandleAsync(CreatePropertyCommand command)
        {
            var propertyResult = PropertyEntity.Create(
                command.Address,
                command.Price,
                command.Description,
                command.Details
            );

            if (propertyResult.IsFailure)
            {
                return Result.Failure<PropertyEntity>(propertyResult.Error);
            }

            var property = propertyResult.Value;
            property.SetFirstOwner(command.OwnerRecord);

            var saveResult = await _propertyRepository.AddAsync(property);
            if (saveResult.IsFailure)
            {
                return Result.Failure<PropertyEntity>(saveResult.Error);
            }

            return Result.Success(property);
        }
    }
}