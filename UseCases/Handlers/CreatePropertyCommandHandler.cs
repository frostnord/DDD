using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Property;
using Domain.Domain.Property.Property;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreatePropertyCommandHandler : ICommandHandler<CreatePropertyCommand, Property>
    {
        private readonly IPropertyRepository _propertyRepository;

        public CreatePropertyCommandHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Result<Property>> HandleAsync(CreatePropertyCommand command)
        {
            var propertyResult = Property.Create(
                command.Address,
                command.Price,
                command.Description,
                command.Details,
                command.OwnerRecord
            );

            if (propertyResult.IsFailure)
            {
                return Result.Failure<Property>(propertyResult.Error);
            }

            var saveResult = await _propertyRepository.AddAsync(propertyResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Property>(saveResult.Error);
            }

            return Result.Success(propertyResult.Value);
        }
    }
}