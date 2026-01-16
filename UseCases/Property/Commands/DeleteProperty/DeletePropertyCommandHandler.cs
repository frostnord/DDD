using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Property.Commands.DeleteProperty;

public class DeletePropertyCommandHandler : ICommandHandler<DeletePropertyCommand>
{
    private readonly IPropertyRepository _propertyRepository;

    public DeletePropertyCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result> HandleAsync(DeletePropertyCommand command)
    {
        var propertyIdVO = PropertyId.Create(command.PropertyId);
        if (propertyIdVO.IsFailure)
            return Result.Failure(propertyIdVO.Error);

        return await _propertyRepository.DeleteAsync(propertyIdVO.Value);
    }
}