using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.CompletedDealDTO;
using UseCases.CompleteDeal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;
using Presenter.Utilities;
using Xunit;

namespace Test.Controllers
{
    public class CompletedDealsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>> _mockCreateCompleteDealHandler;
        private readonly Mock<ICompletedDealService> _mockCompletedDealService;
        private readonly CompletedDealsController _controller;

        public CompletedDealsControllerTests()
        {
            _mockCreateCompleteDealHandler = new Mock<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>>();
            _mockCompletedDealService = new Mock<ICompletedDealService>();
            _controller = new CompletedDealsController(
                _mockCreateCompleteDealHandler.Object,
                _mockCompletedDealService.Object);
        }

        private static CompletedDealEntity CreateCompletedDealEntity(
            Guid? buyerClientId = null, 
            Guid? sellerClientId = null, 
            Guid? propertyId = null, 
            DateTime? dealDate = null, 
            decimal? dealAmount = null, 
            DealType dealType = null)
        {
            var buyerId = ClientId.Create(buyerClientId ?? Guid.NewGuid()).Value;
            var sellerId = ClientId.Create(sellerClientId ?? Guid.NewGuid()).Value;
            var propertyIdValue = PropertyId.Create(propertyId ?? Guid.NewGuid()).Value;
            var dealDateValue = dealDate ?? DateTime.UtcNow.AddDays(-1);
            var dealAmountValue = Price.Create(dealAmount ?? 1000m).Value;
            var dealTypeValue = dealType ?? DealType.Purchase;

            return CompletedDealEntity.Create(buyerId, sellerId, propertyIdValue, dealDateValue, dealAmountValue, dealTypeValue).Value;
        }

        private static IEnumerable<CompletedDealDto> GetItemsFromEnvelope(Envelope envelope)
        {
            var items = Assert.IsAssignableFrom<IEnumerable<CompletedDealDto>>(envelope.Result);
            return items;
        }

        [Fact]
        public async Task CreateCompletedDeal_ValidRequest_ReturnsCreatedEnvelope()
        {
            // Arrange
            var request = new CreateCompletedDealRequest
            {
                BuyerClientId = Guid.NewGuid(),
                SellerClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                DealDate = DateTime.UtcNow.AddDays(-1),
                DealAmount = 1000m,
                DealType = "Purchase"
            };

            var completedDeal = CreateCompletedDealEntity(
                request.BuyerClientId,
                request.SellerClientId,
                request.PropertyId,
                request.DealDate,
                request.DealAmount,
                DealType.Purchase);

            var result = Result.Success(completedDeal);
            _mockCreateCompleteDealHandler.Setup(x => x.HandleAsync(It.IsAny<CreateCompleteDealCommand>()))
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.CreateCompletedDeal(request);

            // Assert
            Assert.Equal(201, envelope.Status); // Created
            var dto = Assert.IsType<CompletedDealDto>(envelope.Result);
            Assert.Equal(completedDeal.Id.Value, dto.Id);
            Assert.Equal(completedDeal.BuyerClientId.Value, dto.BuyerClientId);
            Assert.Equal(completedDeal.SellerClientId.Value, dto.SellerClientId);
            Assert.Equal(completedDeal.PropertyId.Value, dto.PropertyId);
            Assert.Equal(completedDeal.DealDate, dto.DealDate);
            Assert.Equal(completedDeal.DealAmount.Value, dto.DealAmount);
            Assert.Equal(completedDeal.DealType.Name, dto.DealType);
        }

        [Fact]
        public async Task CreateCompletedDeal_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateCompletedDealRequest
            {
                BuyerClientId = Guid.NewGuid(),
                SellerClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                DealDate = DateTime.UtcNow.AddDays(-1),
                DealAmount = 100m,
                DealType = "Purchase"
            };

            var errorResult = Result.Failure<CompletedDealEntity>("Validation error");
            _mockCreateCompleteDealHandler.Setup(x => x.HandleAsync(It.IsAny<CreateCompleteDealCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.CreateCompletedDeal(request);

            // Assert
            Assert.Equal(400, envelope.Status); // BadRequest
            Assert.Contains("Validation error", envelope.Error);
        }

        [Fact]
        public async Task GetCompletedDeal_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var dealIdGuid = Guid.NewGuid();
            var completedDeal = CreateCompletedDealEntity();

            var result = Result.Success(completedDeal);
            _mockCompletedDealService.Setup(x => x.GetByIdAsync(dealIdGuid))
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.GetCompletedDeal(dealIdGuid);

            // Assert
            Assert.Equal(200, envelope.Status); // OK
            var dealDto = Assert.IsType<CompletedDealDto>(envelope.Result);
            Assert.Equal(completedDeal.Id.Value, dealDto.Id);
            Assert.Equal(completedDeal.BuyerClientId.Value, dealDto.BuyerClientId);
            Assert.Equal(completedDeal.SellerClientId.Value, dealDto.SellerClientId);
            Assert.Equal(completedDeal.PropertyId.Value, dealDto.PropertyId);
            Assert.Equal(completedDeal.DealDate, dealDto.DealDate);
            Assert.Equal(completedDeal.DealAmount.Value, dealDto.DealAmount);
            Assert.Equal(completedDeal.DealType.Name, dealDto.DealType);
            _mockCompletedDealService.Verify(x => x.GetByIdAsync(dealIdGuid), Times.Once);
        }

        [Fact]
        public async Task GetCompletedDeal_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure<CompletedDealEntity>("Completed deal not found");
            _mockCompletedDealService.Setup(x => x.GetByIdAsync(dealId))
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.GetCompletedDeal(dealId);

            // Assert
            Assert.Equal(404, envelope.Status); // NotFound
            Assert.Contains("Completed deal not found", envelope.Error);
        }

        [Fact]
        public async Task GetAllCompletedDeals_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var completedDeal = CreateCompletedDealEntity();
            var completedDealsList = new List<CompletedDealEntity> { completedDeal };

            var result = Result.Success<IEnumerable<CompletedDealEntity>>(completedDealsList);
            _mockCompletedDealService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.GetAllCompletedDeals();

            // Assert
            Assert.Equal(200, envelope.Status); // OK
            var items = GetItemsFromEnvelope(envelope);
            var singleItem = Assert.Single(items);
            Assert.Equal(completedDeal.Id.Value, singleItem.Id);
            Assert.Equal(completedDeal.BuyerClientId.Value, singleItem.BuyerClientId);
            Assert.Equal(completedDeal.SellerClientId.Value, singleItem.SellerClientId);
            Assert.Equal(completedDeal.PropertyId.Value, singleItem.PropertyId);
            Assert.Equal(completedDeal.DealDate, singleItem.DealDate);
            Assert.Equal(completedDeal.DealAmount.Value, singleItem.DealAmount);
            Assert.Equal(completedDeal.DealType.Name, singleItem.DealType);
        }

        [Fact]
        public async Task GetAllCompletedDeals_WithEmptyData_ReturnsOkResult()
        {
            // Arrange
            var completedDealsList = new List<CompletedDealEntity>();

            var result = Result.Success<IEnumerable<CompletedDealEntity>>(completedDealsList);
            _mockCompletedDealService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.GetAllCompletedDeals();

            // Assert
            Assert.Equal(200, envelope.Status); // OK
            var items = GetItemsFromEnvelope(envelope);
            Assert.Empty(items);
        }

        [Fact]
        public async Task GetAllCompletedDeals_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var errorResult = Result.Failure<IEnumerable<CompletedDealEntity>>("Service error");
            _mockCompletedDealService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.GetAllCompletedDeals();

            // Assert
            Assert.Equal(400, envelope.Status); // BadRequest
            Assert.Contains("Service error", envelope.Error);
        }

        [Fact]
        public async Task GetCompletedDealsByClient_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var completedDeal = CreateCompletedDealEntity(buyerClientId: clientId);
            var completedDealsList = new List<CompletedDealEntity> { completedDeal };

            var result = Result.Success<IEnumerable<CompletedDealEntity>>(completedDealsList);
            _mockCompletedDealService.Setup(x => x.GetByClientIdAsync(clientId))
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.GetCompletedDealsByClient(clientId);

            // Assert
            Assert.Equal(200, envelope.Status); // OK
            var items = GetItemsFromEnvelope(envelope);
            var singleItem = Assert.Single(items);
            Assert.Equal(completedDeal.Id.Value, singleItem.Id);
            Assert.Equal(completedDeal.BuyerClientId.Value, singleItem.BuyerClientId);
            Assert.Equal(completedDeal.SellerClientId.Value, singleItem.SellerClientId);
            Assert.Equal(completedDeal.PropertyId.Value, singleItem.PropertyId);
        }

        [Fact]
        public async Task GetCompletedDealsByClient_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var errorResult = Result.Failure<IEnumerable<CompletedDealEntity>>("Service error");
            _mockCompletedDealService.Setup(x => x.GetByClientIdAsync(clientId))
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.GetCompletedDealsByClient(clientId);

            // Assert
            Assert.Equal(400, envelope.Status); // BadRequest
            Assert.Contains("Service error", envelope.Error);
        }

        [Fact]
        public async Task GetCompletedDealsByProperty_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var completedDeal = CreateCompletedDealEntity(propertyId: propertyId);
            var completedDealsList = new List<CompletedDealEntity> { completedDeal };

            var result = Result.Success<IEnumerable<CompletedDealEntity>>(completedDealsList);
            _mockCompletedDealService.Setup(x => x.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.GetCompletedDealsByProperty(propertyId);

            // Assert
            Assert.Equal(200, envelope.Status); // OK
            var items = GetItemsFromEnvelope(envelope);
            var singleItem = Assert.Single(items);
            Assert.Equal(completedDeal.Id.Value, singleItem.Id);
            Assert.Equal(completedDeal.BuyerClientId.Value, singleItem.BuyerClientId);
            Assert.Equal(completedDeal.SellerClientId.Value, singleItem.SellerClientId);
            Assert.Equal(completedDeal.PropertyId.Value, singleItem.PropertyId);
        }

        [Fact]
        public async Task GetCompletedDealsByProperty_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var errorResult = Result.Failure<IEnumerable<CompletedDealEntity>>("Service error");
            _mockCompletedDealService.Setup(x => x.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.GetCompletedDealsByProperty(propertyId);

            // Assert
            Assert.Equal(400, envelope.Status); // BadRequest
            Assert.Contains("Service error", envelope.Error);
        }

        [Fact]
        public async Task DeleteCompletedDeal_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var result = Result.Success();
            _mockCompletedDealService.Setup(x => x.DeleteAsync(dealId))
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.DeleteCompletedDeal(dealId);

            // Assert
            Assert.Equal(204, envelope.Status); // NoContent
            _mockCompletedDealService.Verify(x => x.DeleteAsync(dealId), Times.Once);
        }

        [Fact]
        public async Task DeleteCompletedDeal_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure("Completed deal not found");
            _mockCompletedDealService.Setup(x => x.DeleteAsync(dealId))
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.DeleteCompletedDeal(dealId);

            // Assert
            Assert.Equal(404, envelope.Status); // NotFound
            Assert.Contains("Completed deal not found", envelope.Error);
        }
    }
}