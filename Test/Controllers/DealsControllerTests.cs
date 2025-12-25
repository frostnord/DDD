using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.DealDTO;
using Presenter.Utilities;
using UseCases.CompleteDeal;
using UseCases.Deal;
using UseCases.Deal.Commands;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;
using Xunit;

namespace Test.Controllers
{
    public class DealsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateDealCommand, DealEntity>> _mockCreateDealHandler;
        private readonly Mock<ICommandHandler<ConfirmDealCommand>> _mockConfirmDealHandler;
        private readonly Mock<ICommandHandler<CompleteDealCommand>> _mockCompleteDealHandler;
        private readonly Mock<ICommandHandler<CancelDealCommand>> _mockCancelDealHandler;
        private readonly Mock<IDealService> _mockDealService;
        private readonly DealsController _controller;

        public DealsControllerTests()
        {
            _mockCreateDealHandler = new Mock<ICommandHandler<CreateDealCommand, DealEntity>>();
            _mockConfirmDealHandler = new Mock<ICommandHandler<ConfirmDealCommand>>();
            _mockCompleteDealHandler = new Mock<ICommandHandler<CompleteDealCommand>>();
            _mockCancelDealHandler = new Mock<ICommandHandler<CancelDealCommand>>();
            _mockDealService = new Mock<IDealService>();
            _controller = new DealsController(
                _mockCreateDealHandler.Object,
                _mockConfirmDealHandler.Object,
                _mockCompleteDealHandler.Object,
                _mockCancelDealHandler.Object,
                _mockDealService.Object);
        }

        private static DealEntity CreateDealEntity(Guid? clientId = null, Guid? propertyId = null, Guid? bookingId = null, DealDetails details = null)
        {
            var clientIdValue = ClientId.Create(clientId ?? Guid.NewGuid()).Value;
            var propertyIdValue = PropertyId.Create(propertyId ?? Guid.NewGuid()).Value;

            BookingId? bookingIdValue = null;
            if (bookingId.HasValue)
            {
                bookingIdValue = BookingId.Create(bookingId.Value).Value;
            }

            var dealDetails = details ?? DealDetails.Create(
                DateTime.UtcNow,
                Domain.ValueObjects.Price.Create(1000).Value,
                "Test deal",
                null).Value;

            return DealEntity.Create(clientIdValue, propertyIdValue, bookingIdValue, dealDetails).Value;
        }

        private static IEnumerable<DealDto> ExtractItems(dynamic response)
        {
            var itemsProperty = response.GetType().GetProperty("Items", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(itemsProperty);

            var value = itemsProperty.GetValue(response);
            return Assert.IsAssignableFrom<IEnumerable<DealDto>>(value);
        }

        [Fact]
        public async Task CreateDeal_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateDealRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                BookingId = Guid.NewGuid(),
                Details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(1000).Value, "Test deal", null).Value
            };

            var dealId = DealId.Create(Guid.NewGuid()).Value;
            var clientId = ClientId.Create(request.ClientId).Value;
            var propertyId = PropertyId.Create(request.PropertyId).Value;
            var bookingId = request.BookingId.HasValue ? BookingId.Create(request.BookingId.Value).Value : null;
            
            var deal = DealEntity.Create(clientId, propertyId, bookingId, request.Details).Value;
            deal.Confirm(); // Для установки статуса, если нужно

            var result = Result.Success<DealEntity>(deal);
            _mockCreateDealHandler.Setup(x => x.HandleAsync(It.IsAny<CreateDealCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateDeal(request);
            var envelope = Assert.IsType<Envelope>(actionResult);

            // Assert
            Assert.Equal((int)System.Net.HttpStatusCode.Created, envelope.Status);
            var dto = Assert.IsType<DealDto>(envelope.Result);
            Assert.Equal(deal.Id.Value, dto.Id);
            Assert.Equal(deal.ClientId.Value, dto.ClientId);
            Assert.Equal(deal.PropertyId.Value, dto.PropertyId);
            Assert.Equal(deal.BookingId?.Value, dto.BookingId);
        }

        [Fact]
        public async Task CreateDeal_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateDealRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                BookingId = Guid.NewGuid(),
                Details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(1000).Value, "Test deal", null).Value
            };

            var errorResult = Result.Failure<DealEntity>("Validation error");
            _mockCreateDealHandler.Setup(x => x.HandleAsync(It.IsAny<CreateDealCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateDeal(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, envelope.Status);
            Assert.Contains("Validation error", envelope.Error);
        }

        [Fact]
        public async Task GetDeal_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var dealIdGuid = Guid.NewGuid();
            var dealId = DealId.Create(dealIdGuid).Value;
            var clientId = ClientId.Create(Guid.NewGuid()).Value;
            var propertyId = PropertyId.Create(Guid.NewGuid()).Value;
            var bookingId = BookingId.Create(Guid.NewGuid()).Value;
            var details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(100).Value, "Test deal", null).Value;
            
            var deal = DealEntity.Create(clientId, propertyId, bookingId, details).Value;

            var result = Result.Success<DealEntity>(deal);
            _mockDealService.Setup(x => x.GetByIdAsync(dealIdGuid))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetDeal(dealIdGuid);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var dealDto = Assert.IsType<DealDto>(envelope.Result);
            Assert.Equal(deal.Id.Value, dealDto.Id);
            Assert.Equal(deal.ClientId.Value, dealDto.ClientId);
            Assert.Equal(deal.PropertyId.Value, dealDto.PropertyId);
            Assert.Equal(deal.BookingId?.Value, dealDto.BookingId);
            Assert.Equal(deal.Status.Name, dealDto.Status);
            _mockDealService.Verify(x => x.GetByIdAsync(dealIdGuid), Times.Once);
        }

        [Fact]
        public async Task GetDeal_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure<DealEntity>("Deal not found");
            _mockDealService.Setup(x => x.GetByIdAsync(dealId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.NotFound, envelope.Status);
            Assert.Equal("Deal not found", envelope.Error);
        }

        [Fact]
        public async Task GetDeals_WithClientId_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var query = new SearchDealsQuery(clientId, null);
            var details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(100).Value, "Test deal", null).Value;
            var dealId = DealId.Create(Guid.NewGuid()).Value;
            var clientIdVO = ClientId.Create(clientId).Value;
            var propertyIdVO = PropertyId.Create(Guid.NewGuid()).Value;
            var bookingIdVO = BookingId.Create(Guid.NewGuid()).Value;
            
            var deal = DealEntity.Create(clientIdVO, propertyIdVO, bookingIdVO, details).Value;
            
            var dealsList = new List<DealEntity> { deal };

            var result = Result.Success<IEnumerable<DealEntity>>(dealsList);
            _mockDealService.Setup(x => x.GetByClientIdAsync(clientId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetDeals(query);
            var envelope = Assert.IsType<Envelope>(actionResult);

            // Assert
            var response = envelope.Result as dynamic;
            Assert.NotNull(response);
            var items = ExtractItems(response);
            var singleItem = Assert.Single(items);
            Assert.Equal(deal.Id.Value, singleItem.Id);
            Assert.Equal(deal.ClientId.Value, singleItem.ClientId);
            Assert.Equal(deal.PropertyId.Value, singleItem.PropertyId);
        }

        [Fact]
        public async Task GetDeals_WithPropertyId_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var query = new SearchDealsQuery(null, propertyId);
            var details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(100).Value, "Test deal", null).Value;
            var dealId = DealId.Create(Guid.NewGuid()).Value;
            var clientIdVO = ClientId.Create(Guid.NewGuid()).Value;
            var propertyIdVO = PropertyId.Create(propertyId).Value;
            var bookingIdVO = BookingId.Create(Guid.NewGuid()).Value;
            
            var deal = DealEntity.Create(clientIdVO, propertyIdVO, bookingIdVO, details).Value;
            
            var dealsList = new List<DealEntity> { deal };

            var result = Result.Success<IEnumerable<DealEntity>>(dealsList);
            _mockDealService.Setup(x => x.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetDeals(query);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var response = envelope.Result as dynamic;
            Assert.NotNull(response);
            var items = ExtractItems(response);
            var singleItem = Assert.Single(items);
            Assert.Equal(deal.Id.Value, singleItem.Id);
            Assert.Equal(deal.ClientId.Value, singleItem.ClientId);
            Assert.Equal(deal.PropertyId.Value, singleItem.PropertyId);
        }

        [Fact]
        public async Task GetDeals_WithoutFilters_ReturnsBadRequest()
        {
            // Arrange
            var query = new SearchDealsQuery(null, null);

            // Act
            var actionResult = await _controller.GetDeals(query);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, envelope.Status);
            Assert.Contains("Нужен id клиента или недвижимости", envelope.Error);
        }

        [Fact]
        public async Task ConfirmDeal_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var result = Result.Success();
            _mockConfirmDealHandler.Setup(x => x.HandleAsync(It.IsAny<ConfirmDealCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.ConfirmDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.OK, envelope.Status);
            Assert.Contains("Deal confirmed successfully", envelope.Result.ToString());
        }

        [Fact]
        public async Task ConfirmDeal_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockConfirmDealHandler.Setup(x => x.HandleAsync(It.IsAny<ConfirmDealCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.ConfirmDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, envelope.Status);
            Assert.Contains("Validation error", envelope.Error);
        }

        [Fact]
        public async Task CompleteDeal_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var result = Result.Success();
            _mockCompleteDealHandler.Setup(x => x.HandleAsync(It.IsAny<CompleteDealCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CompleteDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.OK, envelope.Status);
            Assert.Contains("Deal completed successfully", envelope.Result.ToString());
        }

        [Fact]
        public async Task CompleteDeal_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockCompleteDealHandler.Setup(x => x.HandleAsync(It.IsAny<CompleteDealCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CompleteDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, envelope.Status);
            Assert.Contains("Validation error", envelope.Error);
        }

        [Fact]
        public async Task CancelDeal_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var result = Result.Success();
            _mockCancelDealHandler.Setup(x => x.HandleAsync(It.IsAny<CancelDealCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CancelDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.OK, envelope.Status);
            Assert.Contains("Deal cancelled successfully", envelope.Result.ToString());
        }

        [Fact]
        public async Task CancelDeal_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockCancelDealHandler.Setup(x => x.HandleAsync(It.IsAny<CancelDealCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CancelDeal(dealId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal((int)System.Net.HttpStatusCode.BadRequest, envelope.Status);
            Assert.Contains("Validation error", envelope.Error);
        }
    }
}