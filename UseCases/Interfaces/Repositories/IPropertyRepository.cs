using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property;
using Domain.Property.VO;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Result<PropertyEntity>> GetByIdAsync(PropertyId id, CancellationToken cancellationToken = default);
    Task<Result<PropertyEntity>> GetByIdForUpdateAsync(PropertyId id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PropertyEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<(IEnumerable<PropertyEntity> Items, int TotalCount)>> SearchAsync(SearchPropertiesQuery query, CancellationToken cancellationToken = default);
    Task<Result<PropertyEntity?>> GetActiveReservationByPropertyIdAsync(PropertyId propertyId, DateTime nowUtc, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PropertyEntity>>> GetActiveReservationByClientIdAsync(ClientId clientId, DateTime nowUtc, CancellationToken cancellationToken = default);
    Result<PropertyEntity> Add(PropertyEntity propertyEntity);
    Result Update(PropertyEntity propertyEntity);
    Result Delete(PropertyId id);
    Task<bool> ExistsAsync(PropertyId id, CancellationToken cancellationToken = default);
}
