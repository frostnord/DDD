using CSharpFunctionalExtensions;
using Domain.Property;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.GetPropertyById
{
    public sealed record GetPropertyByIdQuery(Guid PropertyId) : IQuery<Result<PropertyEntity>>;
    
    
}