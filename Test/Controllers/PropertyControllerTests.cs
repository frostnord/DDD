using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.PropertyDTO;
using Presenter.DTOs.PropertyDTO.Request.CreatePoperty;
using Presenter.DTOs.PropertyDTO.Request.UpdateProperty;
using Presenter.DTOs.PropertyDTO.Response;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Property.Commands;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Commands.DeleteProperty;
using UseCases.Property.Queries;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.Reservation.Queries;
using UseCases.UseCases.DTO.Booking;
using UseCases.UseCases.DTO.Property;
using Xunit;
using AddressDto = Presenter.DTOs.PropertyDTO.AddressDto;
using OwnershipDto = Presenter.DTOs.PropertyDTO.OwnershipDto;
using PropertyDetailsDto = Presenter.DTOs.PropertyDTO.PropertyDetailsDto;

namespace Test.Controllers
{
    public class PropertyControllerTests
    {
        private readonly Mock<ICommandHandler<CreatePropertyCommand, Guid>> _mockCreatePropertyHandler;
        private readonly Mock<ICommandHandler<UpdatePropertyCommand>> _mockUpdatePropertyHandler;
        private readonly Mock<ICommandHandler<DeletePropertyCommand>> _mockDeletePropertyHandler;
        private readonly Mock<IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>>> _mockGetPropertyByIdHandler;
        private readonly Mock<IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>>> _mockSearchPropertiesHandler;
        private readonly Mock<IQueryHandler<GetPropertyReservationQuery, Result<ReservationDto>>> _mockGetPropertyReservationHandler;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PropertyController _controller;

        public PropertyControllerTests()
        {
            _mockCreatePropertyHandler = new Mock<ICommandHandler<CreatePropertyCommand, Guid>>();
            _mockUpdatePropertyHandler = new Mock<ICommandHandler<UpdatePropertyCommand>>();
            _mockDeletePropertyHandler = new Mock<ICommandHandler<DeletePropertyCommand>>();
            _mockGetPropertyByIdHandler = new Mock<IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>>>();
            _mockSearchPropertiesHandler = new Mock<IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>>>();
            _mockGetPropertyReservationHandler = new Mock<IQueryHandler<GetPropertyReservationQuery, Result<ReservationDto>>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PropertyController(
                _mockCreatePropertyHandler.Object,
                _mockUpdatePropertyHandler.Object,
                _mockDeletePropertyHandler.Object,
                _mockGetPropertyByIdHandler.Object,
                _mockSearchPropertiesHandler.Object,
                _mockGetPropertyReservationHandler.Object,
                _mockMapper.Object);
        }

        private PropertyDto CreateTestUseCasePropertyDto(Guid propertyId)
        {
            return new PropertyDto(
                propertyId,
                new UseCases.UseCases.DTO.Property.AddressDto("Main St", "City", 123, 123456, "Country"),
                new UseCases.UseCases.DTO.Property.PropertyDetailsDto(100000, "Nice property", 2, 9, 1, 60.5m, "Apartment", "Central", "Good", true),
                new UseCases.UseCases.DTO.Property.OwnershipDto(Guid.NewGuid(), DateTime.UtcNow)
            );
        }

        private CreatePropertyRequest CreateValidCreateRequest()
        {
            return new CreatePropertyRequest
            {
                Address = new AddressDto { Street = "Test Street", City = "Test City", Country = "Test Country", HomeNumber = 1, ZipCode = 123456 },
                PropertyDetails = new PropertyDetailsDto { Description = "Test Description", Type = "Apartment", HeatingType = "Gas", Condition = "New", Area = 100, Floor = 1, NumberOfRooms = 4, TotalFloors = 10, Price = 100000, HasParking = true },
                Ownership = new OwnershipDto { OwnerClientId = Guid.NewGuid(), StartDate = DateTime.UtcNow }
            };
        }
        
        private UpdatePropertyRequest CreateValidUpdateRequest()
        {
            return new UpdatePropertyRequest
            {
                Address = new AddressDto { Street = "Updated Street", City = "Updated City", Country = "Updated Country", HomeNumber = 2, ZipCode = 654321 },
                PropertyDetails = new PropertyDetailsDto { Description = "Updated Description", Type = "House", HeatingType = "Central", Condition = "Used", Area = 200, Floor = 1, NumberOfRooms = 5, TotalFloors = 2, Price = 200000, HasParking = false },
                Ownership = new OwnershipDto { OwnerClientId = Guid.NewGuid(), StartDate = DateTime.UtcNow }
            };
        }

        [Fact]
        public async Task CreateProperty_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var command = new CreatePropertyCommand(
                new UseCases.UseCases.DTO.Property.AddressDto("Street", "City", 1, 123456, "Country"),
                new UseCases.UseCases.DTO.Property.PropertyDetailsDto(100000m, "Desc", 2, 1, 1, 50m, "Apartment", "Central", "Good", true),
                new UseCases.UseCases.DTO.Property.OwnershipDto(Guid.NewGuid(), DateTime.UtcNow));
            var createdId = Guid.NewGuid();

            _mockMapper.Setup(m => m.Map<CreatePropertyCommand>(request)).Returns(command);
            _mockCreatePropertyHandler.Setup(x => x.HandleAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(createdId));

            // Act
            var result = await _controller.CreateProperty(request, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.Created, envelope.Status);
            Assert.Equal(createdId, envelope.Result);
        }

        [Fact]
        public async Task CreateProperty_HandlerFailure_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var command = new CreatePropertyCommand(
                new UseCases.UseCases.DTO.Property.AddressDto("Street", "City", 1, 123456, "Country"),
                new UseCases.UseCases.DTO.Property.PropertyDetailsDto(100000m, "Desc", 2, 1, 1, 50m, "Apartment", "Central", "Good", true),
                new UseCases.UseCases.DTO.Property.OwnershipDto(Guid.NewGuid(), DateTime.UtcNow));
            var error = "Handler failure";

            _mockMapper.Setup(m => m.Map<CreatePropertyCommand>(request)).Returns(command);
            _mockCreatePropertyHandler.Setup(x => x.HandleAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.CreateProperty(request, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }

        [Fact]
        public async Task GetProperty_ExistingId_ReturnsOk()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var propertyDto = CreateTestUseCasePropertyDto(propertyId);
            var expectedResponse = new PropertyResponse(
                propertyDto.Id,
                new AddressDto
                {
                    Street = propertyDto.AddressDto.Street,
                    City = propertyDto.AddressDto.City,
                    HomeNumber = propertyDto.AddressDto.HomeNumber,
                    ZipCode = propertyDto.AddressDto.ZipCode,
                    Country = propertyDto.AddressDto.Country
                },
                new PropertyDetailsDto
                {
                    Price = propertyDto.PropertyDetailsDto.Price,
                    Description = propertyDto.PropertyDetailsDto.Description,
                    NumberOfRooms = propertyDto.PropertyDetailsDto.NumberOfRooms,
                    Floor = propertyDto.PropertyDetailsDto.Floor,
                    TotalFloors = propertyDto.PropertyDetailsDto.TotalFloors,
                    Area = propertyDto.PropertyDetailsDto.Area,
                    Type = propertyDto.PropertyDetailsDto.Type,
                    HeatingType = propertyDto.PropertyDetailsDto.HeatingType,
                    Condition = propertyDto.PropertyDetailsDto.Condition,
                    HasParking = propertyDto.PropertyDetailsDto.HasParking
                },
                new OwnershipDto
                {
                    OwnerClientId = propertyDto.OwnershipDto.OwnerClientId,
                    StartDate = propertyDto.OwnershipDto.StartDate
                }
            );
            _mockGetPropertyByIdHandler.Setup(x => x.HandleAsync(It.Is<GetPropertyByIdQuery>(q => q.PropertyId == propertyId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(propertyDto));
            _mockMapper.Setup(m => m.Map<PropertyResponse>(propertyDto)).Returns(expectedResponse);

            // Act
            var result = await _controller.GetProperty(propertyId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.OK, envelope.Status);
            Assert.Equal(expectedResponse, envelope.Result);
        }

        [Fact]
        public async Task GetProperty_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var error = "Property not found";
            _mockGetPropertyByIdHandler.Setup(x => x.HandleAsync(It.Is<GetPropertyByIdQuery>(q => q.PropertyId == propertyId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<PropertyDto>(error));

            // Act
            var result = await _controller.GetProperty(propertyId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }

        [Fact]
        public async Task UpdateProperty_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var request = CreateValidUpdateRequest();
            var command = new UpdatePropertyCommand(
                propertyId,
                new UseCases.UseCases.DTO.Property.AddressDto("Street", "City", 1, 123456, "Country"),
                new UseCases.UseCases.DTO.Property.PropertyDetailsDto(100000m, "Desc", 2, 1, 1, 50m, "Apartment", "Central", "Good", true),
                new UseCases.UseCases.DTO.Property.OwnershipDto(Guid.NewGuid(), DateTime.UtcNow));

            _mockMapper
                .Setup(m => m.Map<UpdatePropertyCommand>(It.IsAny<UpdatePropertyRequest>(), It.IsAny<Action<IMappingOperationOptions>>()))
                .Returns(command);
            _mockUpdatePropertyHandler.Setup(x => x.HandleAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.UpdateProperty(propertyId, request, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, envelope.Status);
        }

        [Fact]
        public async Task GetProperties_ValidQuery_ReturnsOk()
        {
            // Arrange
            var query = new SearchPropertiesQuery { Page = 1 }; // Указываем Page, чтобы CurrentPage был 1
            var propertyDto = CreateTestUseCasePropertyDto(Guid.NewGuid());
            var searchResult = new SearchPropertiesQueryResponse(new List<PropertyDto> { propertyDto }, 1, 10, 1);

            var mappedItems = new List<PropertyResponse>
            {
                new PropertyResponse(
                    propertyDto.Id,
                    new AddressDto
                    {
                        Street = propertyDto.AddressDto.Street,
                        City = propertyDto.AddressDto.City,
                        HomeNumber = propertyDto.AddressDto.HomeNumber,
                        ZipCode = propertyDto.AddressDto.ZipCode,
                        Country = propertyDto.AddressDto.Country
                    },
                    new PropertyDetailsDto
                    {
                        Price = propertyDto.PropertyDetailsDto.Price,
                        Description = propertyDto.PropertyDetailsDto.Description,
                        NumberOfRooms = propertyDto.PropertyDetailsDto.NumberOfRooms,
                        Floor = propertyDto.PropertyDetailsDto.Floor,
                        TotalFloors = propertyDto.PropertyDetailsDto.TotalFloors,
                        Area = propertyDto.PropertyDetailsDto.Area,
                        Type = propertyDto.PropertyDetailsDto.Type,
                        HeatingType = propertyDto.PropertyDetailsDto.HeatingType,
                        Condition = propertyDto.PropertyDetailsDto.Condition,
                        HasParking = propertyDto.PropertyDetailsDto.HasParking
                    },
                    new OwnershipDto
                    {
                        OwnerClientId = propertyDto.OwnershipDto.OwnerClientId,
                        StartDate = propertyDto.OwnershipDto.StartDate
                    }
                )
            };

            _mockSearchPropertiesHandler
                .Setup(x => x.HandleAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(searchResult));
            _mockMapper.Setup(m => m.Map<IEnumerable<PropertyResponse>>(searchResult.Items)).Returns(mappedItems);

            // Act
            var result = await _controller.GetProperties(query, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.OK, envelope.Status);
            
            var response = Assert.IsType<PagedPropertiesResponse>(envelope.Result);
            Assert.Equal(mappedItems, response.Items);
            Assert.Equal(searchResult.TotalCount, response.TotalCount);
            Assert.Equal(searchResult.PageSize, response.PageSize);
            Assert.Equal(searchResult.TotalPages, response.TotalPages);
            Assert.Equal(query.Page, response.CurrentPage);
        }

        [Fact]
        public async Task DeleteProperty_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var command = new DeletePropertyCommand(propertyId);

            _mockDeletePropertyHandler
                .Setup(x => x.HandleAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.DeleteProperty(propertyId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, envelope.Status);
        }

        [Fact]
        public async Task DeleteProperty_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var command = new DeletePropertyCommand(propertyId);
            var error = "Property not found";

            _mockDeletePropertyHandler
                .Setup(x => x.HandleAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure(error));

            // Act
            var result = await _controller.DeleteProperty(propertyId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }
    }
}