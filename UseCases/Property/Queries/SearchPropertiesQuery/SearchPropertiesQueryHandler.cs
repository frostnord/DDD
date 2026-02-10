using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.SearchPropertiesQuery;

public class SearchPropertiesQueryHandler : IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchPropertiesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SearchPropertiesQueryResponse>> HandleAsync(SearchPropertiesQuery query)
    {
        var searchResult = await _unitOfWork.Properties.SearchAsync(query);
        if (searchResult.IsFailure)
            return Result.Failure<SearchPropertiesQueryResponse>(searchResult.Error);

        var (entities, totalCount) = searchResult.Value;
        var pagedProperties = entities.ToList();

        var items = pagedProperties.Select(p => new PropertyDto(
            p.Id.Value,
            new AddressDto(
                p.Address.Street,
                p.Address.City,
                p.Address.HomeNumber,
                p.Address.ZipCode,
                p.Address.Country
            ),
            new PropertyDetailsDto(
                p.Price.Value,
                p.Description.Value,
                p.PropertyDetails.NumberOfRooms.Value,
                p.PropertyDetails.Floor.Value,
                p.PropertyDetails.TotalFloors.Value,
                p.PropertyDetails.Area.Value,
                p.PropertyDetails.Type.Name,
                p.PropertyDetails.HeatingType.Value.ToString(),
                p.PropertyDetails.Condition.Value,
                p.PropertyDetails.HasParking
            ),
            new OwnershipDto(
                p.GetCurrentOwner()?.OwnerClientId?.Value ?? Guid.Empty,
                p.GetCurrentOwner()?.StartDate ?? DateTime.MinValue
            )
        )).ToList();

        var response = new SearchPropertiesQueryResponse(
            items,
            totalCount,
            query.PageSize,
            (int)System.Math.Ceiling((double)totalCount / query.PageSize)
        );

        return Result.Success(response);
    }
}