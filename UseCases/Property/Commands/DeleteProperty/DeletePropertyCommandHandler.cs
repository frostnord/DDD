using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Property.Commands.DeleteProperty;

public class DeletePropertyCommandHandler : ICommandHandler<DeletePropertyCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeletePropertyCommand command)
    {
        var propertyIdVO = PropertyId.Create(command.PropertyId);
        if (propertyIdVO.IsFailure)
            return Result.Failure(propertyIdVO.Error);

        var deleteResult = _unitOfWork.Properties.Delete(propertyIdVO.Value);
        if (deleteResult.IsFailure)
        {
            return Result.Failure(deleteResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
