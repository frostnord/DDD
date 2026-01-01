using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.SearchPropertiesQuery;

    public sealed record SearchPropertiesQuery(
        string? City,
        string? PropertyType,
        decimal? MinPrice,
        decimal? MaxPrice,
        int? MinArea,
        int? MaxArea,
        int? MinRooms,
        int? MaxRooms,
        int? MinFloor,
        int? MaxFloor,
        string? HeatingType,
        string? PropertyCondition,
        bool? HasParking,
        int Page,
        int PageSize,
        string? SortBy,
        string SortOrder
    ) : IQuery<SearchPropertiesQueryResponse>, IQuery<Result<SearchPropertiesQueryResponse>>;
    
    public sealed record SearchPropertiesQueryResponse(
        IEnumerable<PropertyDto> Items,
        int TotalCount,
        int PageSize,
        int TotalPages
    );
