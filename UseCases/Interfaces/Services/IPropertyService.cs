using CSharpFunctionalExtensions;
using Domain.Property;

namespace UseCases.Interfaces.Services
{
    public interface IPropertyService
    {

        Task<Result<PropertyEntity>> GetPropertyByIdAsync(Guid propertyId);
        Task<Result<IEnumerable<PropertyEntity>>> GetAllPropertiesAsync();

        Task<Result> UpdatePropertyAsync(Guid propertyId, string street, string city, int homeNumber, int zipCode,
            string country, decimal price, string description, int numberOfRooms, int floor, int totalFloors,
            string propertyType, string heatingType, string propertyCondition, double area, bool? hasParking);

        Task<Result> DeletePropertyAsync(Guid propertyId);

        Task<Result<IEnumerable<PropertyEntity>>> SearchPropertiesAsync(string? city, string? propertyType, decimal? minPrice,
            decimal? maxPrice, int? minArea, int? maxArea, int? minRooms, int? maxRooms, int? minFloor, int? maxFloor,
            string? heatingType, string? propertyCondition, bool? hasParking);
    }
}