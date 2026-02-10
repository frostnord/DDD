using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Result<PropertyEntity>> GetByIdAsync(PropertyId id);
    Task<Result<IEnumerable<PropertyEntity>>> GetAllAsync();
    Task<Result<(IEnumerable<PropertyEntity> Items, int TotalCount)>> SearchAsync(SearchPropertiesQuery query);
    Result<PropertyEntity> Add(PropertyEntity propertyEntity);
    Result Update(PropertyEntity propertyEntity);
    Result Delete(PropertyId id);
    Task<bool> ExistsAsync(PropertyId id);
}
