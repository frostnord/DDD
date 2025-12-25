using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs;
using Presenter.DTOs.PropertyDTO;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;
using UseCases.Property;
using Xunit;

namespace Test.Controllers
{
    public class PropertyControllerTests
    {
        private readonly Mock<ICommandHandler<CreatePropertyCommand, Guid>> _mockCreatePropertyHandler;
        private readonly Mock<IPropertyService> _mockPropertyService;
        private readonly PropertyController _controller;

        // Константы удалены, используем значения из request

        public PropertyControllerTests()
        {
            _mockCreatePropertyHandler = new Mock<ICommandHandler<CreatePropertyCommand, Guid>>();
            _mockPropertyService = new Mock<IPropertyService>();
            _controller = new PropertyController(_mockCreatePropertyHandler.Object, _mockPropertyService.Object);
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
                Area = new decimal(60.5),
                HasParking = true,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            };

            // Создаем реальный объект Property для теста
            var property = CreateTestProperty(request);

            var createdId = property.Id.Value;
            var result = Result.Success(createdId);
            _mockCreatePropertyHandler
                .Setup(x => x.HandleAsync(It.IsAny<CreatePropertyCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateProperty(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(201, envelope.Status);
            var returnedId = Assert.IsType<Guid>(envelope.Result);
            Assert.Equal(createdId, returnedId);
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
                Area = new decimal(60.5),
                HasParking = true,
                OwnerClientId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow
            };

            var errorResult = Result.Failure<Guid>("Street is required");
            _mockCreatePropertyHandler
                .Setup(x => x.HandleAsync(It.IsAny<CreatePropertyCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateProperty(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Street is required", envelope.Error.ToString());
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
                Area = new decimal(60.5),
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            var propertyDto = Assert.IsType<PropertyDto>(envelope.Result);
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(404, envelope.Status);
            Assert.Contains("Property not found", envelope.Error.ToString());
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
                Area = new decimal(80.0),
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
                Area = new decimal(60.5),
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            var propertyDto = Assert.IsType<PropertyDto>(envelope.Result);
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
                    Area = new decimal(60.5),
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            var propertiesDto = Assert.IsType<List<PropertyDto>>(envelope.Result);
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(204, envelope.Status);
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(404, envelope.Status);
            Assert.Contains("Property not found", envelope.Error.ToString());
        }
    }
}