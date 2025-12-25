using System.Reflection;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs;
using Presenter.DTOs.PropertyDTO;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;
using Xunit;

namespace Test.Controllers
{
    public class PropertyControllerTests
    {
        private readonly Mock<IPropertyService> _mockPropertyService;
        private readonly PropertyController _controller;

        // Константы удалены, используем значения из request

        public PropertyControllerTests()
        {
            _mockPropertyService = new Mock<IPropertyService>();
            _controller = new PropertyController(_mockPropertyService.Object);
        }

        private PropertyEntity CreateTestProperty(CreatePropertyRequest request, PropertyId propertyId = null)
        {
            var address = Address.Create(request.Street, request.City, request.HomeNumber, request.ZipCode,
                request.Country).Value;
            var price = Price.Create(request.Price).Value;
            var description = Description.Create(request.Description).Value;
            var propertyType = SmartPropertyType.FromName(request.PropertyType);
            var heatingType = HeatingType.Create(request.HeatingType).Value;
            var propertyCondition = PropertyCondition.Create(request.PropertyCondition).Value;
            var details = PropertyDetails.Create((int)request.Area, request.NumberOfRooms, request.Floor,
                request.TotalFloors, propertyType, false, request.HasParking ?? false, request.HeatingType,
                request.PropertyCondition).Value;

            if (propertyId != null)
            {
                // Создаем Property с нужным ID
                var propertyConstructor =
                    typeof(PropertyEntity).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();
                var status = PropertyStatus.FromName("ForSale");
                return (PropertyEntity)propertyConstructor.Invoke(new object[]
                    { propertyId, address, price, description, details, status });
            }
            else
            {
                return PropertyEntity.Create(address, price, description, details).Value;
            }
        }

        private PropertyEntity CreateTestPropertyWithId(Guid guid, CreatePropertyRequest request)
        {
            var propertyId = PropertyId.Create(guid).Value;
            return CreateTestProperty(request, propertyId);
        }

        [Fact]
        public async Task CreateProperty_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreatePropertyRequest
            {
                Street = "Main St",
                City = "City",
                HomeNumber = 123,
                ZipCode = 123456,
                Country = "Country",
                Price = 100000,
                Description = "Nice property",
                NumberOfRooms = 2,
                Floor = 3,
                TotalFloors = 9,
                PropertyType = "Apartment",
                HeatingType = "Central",
                PropertyCondition = "Good",
                Area = 60.5,
                HasParking = true,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            };

            // Создаем реальный объект Property для теста
            var propertyId = PropertyId.Create(Guid.NewGuid()).Value;
            var property = CreateTestProperty(request);

            var result = Result.Success(property);
            _mockPropertyService.Setup(x => x.CreatePropertyAsync(
                    request.Street, request.City, request.HomeNumber, request.ZipCode, request.Country,
                    request.Price, request.Description,
                    request.NumberOfRooms, request.Floor, request.TotalFloors,
                    request.PropertyType, request.HeatingType, request.PropertyCondition,
                    request.Area, request.HasParking, request.OwnerClientId, request.StartDate))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateProperty(request);

            // Assert
            var actionResultValue = actionResult.Result;
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResultValue);
            Assert.Equal("GetProperty", createdAtResult.ActionName);
        }

        [Fact]
        public async Task CreateProperty_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreatePropertyRequest
            {
                Street = "", // Невалидная улица
                City = "City",
                HomeNumber = 123,
                ZipCode = 123456,
                Country = "Country",
                Price = 100000,
                Description = "Nice property",
                NumberOfRooms = 2,
                Floor = 3,
                TotalFloors = 9,
                PropertyType = "Apartment",
                HeatingType = "Central",
                PropertyCondition = "Good",
                Area = 60.5,
                HasParking = true,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            };

            var errorResult = Result.Failure<PropertyEntity>("Street is required");
            _mockPropertyService.Setup(x => x.CreatePropertyAsync(
                    request.Street, request.City, request.HomeNumber, request.ZipCode, request.Country,
                    request.Price, request.Description,
                    request.NumberOfRooms, request.Floor, request.TotalFloors,
                    request.PropertyType, request.HeatingType, request.PropertyCondition,
                    request.Area, request.HasParking, request.OwnerClientId, request.StartDate))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateProperty(request);

            // Assert
            var actionResultValue = actionResult.Result;
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResultValue);
            Assert.Contains("Street is required", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetProperty_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var request = new CreatePropertyRequest
            {
                Street = "Main St",
                City = "City",
                HomeNumber = 123,
                ZipCode = 123456,
                Country = "Country",
                Price = 100000,
                Description = "Nice property",
                NumberOfRooms = 2,
                Floor = 3,
                TotalFloors = 9,
                PropertyType = "Apartment",
                HeatingType = "Central",
                PropertyCondition = "Good",
                Area = 60.5,
                HasParking = true,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            };

            // Создаем реальный объект Property для теста
            var property = CreateTestPropertyWithId(propertyId, request);

            var result = Result.Success(property);
            _mockPropertyService.Setup(x => x.GetPropertyByIdAsync(propertyId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetProperty(propertyId);

            // Assert
            var actionResultValue = actionResult.Result;
            var okResult = Assert.IsType<OkObjectResult>(actionResultValue);
            var propertyDto = Assert.IsType<PropertyDto>(okResult.Value);
            Assert.Equal(propertyId, propertyDto.Id);
        }

        [Fact]
        public async Task GetProperty_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var errorResult = Result.Failure<PropertyEntity>("Property not found");
            _mockPropertyService.Setup(x => x.GetPropertyByIdAsync(propertyId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetProperty(propertyId);

            // Assert
            var actionResultValue = actionResult.Result;
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResultValue);
            Assert.Contains("Property not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateProperty_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var request = new UpdatePropertyRequest
            {
                Street = "Updated St",
                City = "Updated City",
                HomeNumber = 456,
                ZipCode = 654321,
                Country = "Updated Country",
                Price = 150000,
                Description = "Updated property",
                NumberOfRooms = 3,
                Floor = 5,
                TotalFloors = 12,
                PropertyType = "House",
                HeatingType = "Gas",
                PropertyCondition = "Excellent",
                Area = 80.0,
                HasParking = false
            };

            var originalPropertyRequest = new CreatePropertyRequest
            {
                Street = "Main St",
                City = "City",
                HomeNumber = 123,
                ZipCode = 123456,
                Country = "Country",
                Price = 100000,
                Description = "Nice property",
                NumberOfRooms = 2,
                Floor = 3,
                TotalFloors = 9,
                PropertyType = "Apartment",
                HeatingType = "Central",
                PropertyCondition = "Good",
                Area = 60.5,
                HasParking = true,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            };

            var originalProperty = CreateTestPropertyWithId(propertyId, originalPropertyRequest);
            var updatedProperty = CreateTestPropertyWithId(propertyId, new CreatePropertyRequest
            {
                Street = request.Street,
                City = request.City,
                HomeNumber = request.HomeNumber,
                ZipCode = request.ZipCode,
                Country = request.Country,
                Price = request.Price,
                Description = request.Description,
                NumberOfRooms = request.NumberOfRooms,
                Floor = request.Floor,
                TotalFloors = request.TotalFloors,
                PropertyType = request.PropertyType,
                HeatingType = request.HeatingType,
                PropertyCondition = request.PropertyCondition,
                Area = request.Area,
                HasParking = request.HasParking ?? false,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            });

            var getPropertyResult = Result.Success(originalProperty);
            var updateResult = Result.Success();
            var getResultAfterUpdate = Result.Success(updatedProperty);

            _mockPropertyService.Setup(x => x.GetPropertyByIdAsync(propertyId))
                .ReturnsAsync(getPropertyResult);

            _mockPropertyService.Setup(x => x.UpdatePropertyAsync(
                    propertyId,
                    request.Street, request.City, request.HomeNumber, request.ZipCode, request.Country,
                    request.Price, request.Description, request.NumberOfRooms, request.Floor, request.TotalFloors,
                    request.PropertyType, request.HeatingType, request.PropertyCondition, request.Area,
                    request.HasParking))
                .ReturnsAsync(updateResult);

            _mockPropertyService.Setup(x => x.GetPropertyByIdAsync(propertyId))
                .ReturnsAsync(getResultAfterUpdate);

            // Act
            var actionResult = await _controller.UpdateProperty(propertyId, request);

            // Assert
            var actionResultValue = actionResult.Result;
            var okResult = Assert.IsType<OkObjectResult>(actionResultValue);
            var propertyDto = Assert.IsType<PropertyDto>(okResult.Value);
            Assert.Equal(propertyId, propertyDto.Id);
        }

        [Fact]
        public async Task GetProperties_ValidQuery_ReturnsOkResult()
        {
            // Arrange
            var query = new SearchPropertiesQuery
            {
                City = "City",
                PropertyType = "Apartment",
                MinPrice = 50000,
                MaxPrice = 200000
            };

            var properties = new List<PropertyEntity>
            {
                CreateTestProperty(new CreatePropertyRequest
                {
                    Street = "Main St",
                    City = "City",
                    HomeNumber = 123,
                    ZipCode = 123456,
                    Country = "Country",
                    Price = 100000,
                    Description = "Nice property",
                    NumberOfRooms = 2,
                    Floor = 3,
                    TotalFloors = 9,
                    PropertyType = "Apartment",
                    HeatingType = "Central",
                    PropertyCondition = "Good",
                    Area = 60.5,
                    HasParking = true,
                    OwnerClientId = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow
                })
            };

            var result = Result.Success((IEnumerable<PropertyEntity>)properties);
            _mockPropertyService.Setup(x => x.SearchPropertiesAsync(
                    query.City, query.PropertyType, query.MinPrice, query.MaxPrice, query.MinArea, query.MaxArea,
                    query.MinRooms, query.MaxRooms, query.MinFloor, query.MaxFloor, query.HeatingType,
                    query.PropertyCondition, query.HasParking))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetProperties(query);

            // Assert
            var actionResultValue = actionResult.Result;
            var okResult = Assert.IsType<OkObjectResult>(actionResultValue);
            var propertiesDto = Assert.IsType<List<PropertyDto>>(okResult.Value);
            Assert.Single(propertiesDto);
        }

        [Fact]
        public async Task DeleteProperty_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var result = Result.Success();
            _mockPropertyService.Setup(x => x.DeletePropertyAsync(propertyId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.DeleteProperty(propertyId);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task DeleteProperty_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var errorResult = Result.Failure("Property not found");
            _mockPropertyService.Setup(x => x.DeletePropertyAsync(propertyId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.DeleteProperty(propertyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Contains("Property not found", notFoundResult.Value.ToString());
        }
    }
}