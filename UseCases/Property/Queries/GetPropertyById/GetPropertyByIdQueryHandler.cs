using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.GetPropertyById;

public class GetPropertyByIdQueryHandler : IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<PropertyDto>> HandleAsync(GetPropertyByIdQuery query)
    {
        var propertyDto = await _propertyRepository.GetDtoByIdAsync(query.PropertyId);
        if (propertyDto == null)
            return Result.Failure<PropertyDto>($"Property with ID {query.PropertyId} not found");
            
        return Result.Success(propertyDto);
    }
}