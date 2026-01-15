using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Result<PropertyEntity>> GetByIdAsync(PropertyId id);
    Task<Result<IEnumerable<PropertyEntity>>> GetAllAsync();
    Task<Result<PropertyEntity>> AddAsync(PropertyEntity propertyEntity);
    Task<Result> UpdateAsync(PropertyEntity propertyEntity);
    Task<Result> DeleteAsync(PropertyId id);
    Task<bool> ExistsAsync(PropertyId id);
}