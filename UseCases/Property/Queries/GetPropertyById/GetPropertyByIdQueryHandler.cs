using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.GetPropertyById;

public class GetPropertyByIdQueryHandler : IQueryHandler<GetPropertyByIdQuery, Result<PropertyEntity>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<PropertyEntity>> HandleAsync(GetPropertyByIdQuery query)
    {
        var propertyIdResult = PropertyId.Create(query.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<PropertyEntity>(propertyIdResult.Error);
        }

        var property = await _propertyRepository.GetByIdAsync(propertyIdResult.Value);
        if (property.Value == null)
            return Result.Failure<PropertyEntity>($"Property with ID {query.PropertyId} not found");
            
        return Result.Success(property.Value);
    }
}