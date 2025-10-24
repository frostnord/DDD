using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Property;
using Domain.Domain.Property.Property;
using Domain.Domain.Property.Property.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Interfaces.Repositories
{
    public interface IPropertyRepository
    {
        Task<Result<Property>> GetByIdAsync(PropertyId id);
        Task<Result<IEnumerable<Property>>> GetAllAsync();
        Task<Result<Property>> AddAsync(Property property);
        Task<Result> UpdateAsync(Property property);
        Task<Result> DeleteAsync(PropertyId id);
        Task<bool> ExistsAsync(PropertyId id);
    }
}