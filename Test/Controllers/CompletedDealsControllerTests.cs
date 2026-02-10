using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.CompletedDealDTO;
using Presenter.Utilities;
using UseCases.CompleteDeal;
using UseCases.CompleteDeal.Commands.CreateCompliteDealCommand;
using UseCases.CompleteDeal.Commands.DeleteCompletedDeal;
using UseCases.CompleteDeal.Queries.GetAllCompletedDeals;
using UseCases.CompleteDeal.Queries.GetCompletedDealById;
using UseCases.CompleteDeal.Queries.GetCompletedDealsByClientId;
using UseCases.CompleteDeal.Queries.GetCompletedDealsByPropertyId;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCasesCompletedDealDto = UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto;

using Xunit;

namespace Test.Controllers
{
    public class CompletedDealsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>> _mockCreateCompleteDealHandler;
        private readonly Mock<ICommandHandler<DeleteCompletedDealCommand>> _mockDeleteCompletedDealHandler;
        private readonly Mock<IQueryHandler<GetCompletedDealByIdQuery, Result<UseCasesCompletedDealDto>>> _mockGetCompletedDealByIdHandler;
        private readonly Mock<IQueryHandler<GetAllCompletedDealsQuery, Result<IEnumerable<UseCasesCompletedDealDto>>>> _mockGetAllCompletedDealsHandler;
        private readonly Mock<IQueryHandler<GetCompletedDealsByClientIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>>> _mockGetCompletedDealsByClientIdHandler;
        private readonly Mock<IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>>> _mockGetCompletedDealsByPropertyIdHandler;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CompletedDealsController _controller;

        public CompletedDealsControllerTests()
        {
            _mockCreateCompleteDealHandler = new Mock<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>>();
            _mockDeleteCompletedDealHandler = new Mock<ICommandHandler<DeleteCompletedDealCommand>>();
            _mockGetCompletedDealByIdHandler = new Mock<IQueryHandler<GetCompletedDealByIdQuery, Result<UseCasesCompletedDealDto>>>();
            _mockGetAllCompletedDealsHandler = new Mock<IQueryHandler<GetAllCompletedDealsQuery, Result<IEnumerable<UseCasesCompletedDealDto>>>>();
            _mockGetCompletedDealsByClientIdHandler = new Mock<IQueryHandler<GetCompletedDealsByClientIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>>>();
            _mockGetCompletedDealsByPropertyIdHandler = new Mock<IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CompletedDealsController(
                _mockCreateCompleteDealHandler.Object,
                _mockDeleteCompletedDealHandler.Object,
                _mockGetCompletedDealByIdHandler.Object,
                _mockGetAllCompletedDealsHandler.Object,
                _mockGetCompletedDealsByClientIdHandler.Object,
                _mockGetCompletedDealsByPropertyIdHandler.Object,
                _mockMapper.Object);
        }

        private static CompletedDealEntity CreateCompletedDealEntity(
            Guid? buyerClientId = null, 
            Guid? sellerClientId = null, 
            Guid? propertyId = null, 
            DateTime? dealDate = null, 
            decimal? dealAmount = null, 
            DealType? dealType = null)
        {
            var buyerClientIdValue = ClientId.Create(buyerClientId ?? Guid.NewGuid()).Value;
            var sellerClientIdValue = ClientId.Create(sellerClientId ?? Guid.NewGuid()).Value;
            var propertyIdValue = PropertyId.Create(propertyId ?? Guid.NewGuid()).Value;
            var dealDateValue = dealDate ?? DateTime.UtcNow.AddDays(-1);
            var dealAmountValue = Price.Create(dealAmount ?? 1000m).Value;
            var dealTypeValue = dealType ?? DealType.Purchase;

            var buyerRoleIdValue = BuyerId.Create(Guid.NewGuid()).Value;
            var sellerRoleIdValue = SellerId.Create(Guid.NewGuid()).Value;

            return CompletedDealEntity.Create(
                buyerRoleIdValue,
                sellerRoleIdValue,
                buyerClientIdValue,
                sellerClientIdValue,
                propertyIdValue,
                dealDateValue,
                dealAmountValue,
                dealTypeValue).Value;
        }

        private static IEnumerable<CompletedDealDto> GetItemsFromEnvelope(Envelope envelope)
        {
            var items = Assert.IsAssignableFrom<IEnumerable<CompletedDealDto>>(envelope.Result);
            return items;
        }

        private static UseCasesCompletedDealDto CreateUseCasesCompletedDealDto(CompletedDealEntity entity)
        {
            return new UseCasesCompletedDealDto(
                entity.Id.Value,
                entity.BuyerClientId.Value,
                entity.SellerClientId.Value,
                entity.PropertyId.Value,
                entity.DealDate,
                entity.DealAmount.Value,
                entity.DealType.Name,
                entity.CreatedAt,
                entity.UpdatedAt);
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
            _mockMapper.Setup(m => m.Map<CreateCompleteDealCommand>(request))
                .Returns(new CreateCompleteDealCommand(
                    request.BuyerClientId,
                    request.SellerClientId,
                    request.PropertyId,
                    request.DealDate,
                    request.DealAmount,
                    request.DealType));
            _mockMapper.Setup(m => m.Map<CompletedDealDto>(completedDeal))
                .Returns(new CompletedDealDto(
                    completedDeal.Id.Value,
                    completedDeal.BuyerClientId.Value,
                    completedDeal.SellerClientId.Value,
                    completedDeal.PropertyId.Value,
                    completedDeal.DealDate,
                    completedDeal.DealAmount.Value,
                    completedDeal.DealType.Name,
                    completedDeal.CreatedAt,
                    completedDeal.UpdatedAt));

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
            _mockMapper.Setup(m => m.Map<CreateCompleteDealCommand>(request))
                .Returns(new CreateCompleteDealCommand(
                    request.BuyerClientId,
                    request.SellerClientId,
                    request.PropertyId,
                    request.DealDate,
                    request.DealAmount,
                    request.DealType));

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
            var useCasesDto = CreateUseCasesCompletedDealDto(completedDeal);
            var responseDto = new CompletedDealDto(
                completedDeal.Id.Value,
                completedDeal.BuyerClientId.Value,
                completedDeal.SellerClientId.Value,
                completedDeal.PropertyId.Value,
                completedDeal.DealDate,
                completedDeal.DealAmount.Value,
                completedDeal.DealType.Name,
                completedDeal.CreatedAt,
                completedDeal.UpdatedAt);

            var result = Result.Success(useCasesDto);
            _mockGetCompletedDealByIdHandler.Setup(x => x.HandleAsync(It.Is<GetCompletedDealByIdQuery>(q => q.CompletedDealId == dealIdGuid)))
                .ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<CompletedDealDto>(useCasesDto)).Returns(responseDto);

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
            _mockGetCompletedDealByIdHandler.Verify(x => x.HandleAsync(It.Is<GetCompletedDealByIdQuery>(q => q.CompletedDealId == dealIdGuid)), Times.Once);
        }

        [Fact]
        public async Task GetCompletedDeal_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure<UseCasesCompletedDealDto>("Completed deal not found");
            _mockGetCompletedDealByIdHandler.Setup(x => x.HandleAsync(It.Is<GetCompletedDealByIdQuery>(q => q.CompletedDealId == dealId)))
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
            var useCasesDeal = CreateUseCasesCompletedDealDto(completedDeal);
            var completedDealsList = new List<UseCasesCompletedDealDto> { useCasesDeal };
            var mapped = new List<CompletedDealDto>
            {
                new CompletedDealDto(
                    completedDeal.Id.Value,
                    completedDeal.BuyerClientId.Value,
                    completedDeal.SellerClientId.Value,
                    completedDeal.PropertyId.Value,
                    completedDeal.DealDate,
                    completedDeal.DealAmount.Value,
                    completedDeal.DealType.Name,
                    completedDeal.CreatedAt,
                    completedDeal.UpdatedAt)
            };

            var result = Result.Success<IEnumerable<UseCasesCompletedDealDto>>(completedDealsList);
            _mockGetAllCompletedDealsHandler.Setup(x => x.HandleAsync(It.IsAny<GetAllCompletedDealsQuery>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompletedDealDto>>(completedDealsList)).Returns(mapped);

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
            var completedDealsList = new List<UseCasesCompletedDealDto>();
            var mapped = new List<CompletedDealDto>();

            var result = Result.Success<IEnumerable<UseCasesCompletedDealDto>>(completedDealsList);
            _mockGetAllCompletedDealsHandler.Setup(x => x.HandleAsync(It.IsAny<GetAllCompletedDealsQuery>()))
                .ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompletedDealDto>>(completedDealsList)).Returns(mapped);

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
            var errorResult = Result.Failure<IEnumerable<UseCasesCompletedDealDto>>("Service error");
            _mockGetAllCompletedDealsHandler.Setup(x => x.HandleAsync(It.IsAny<GetAllCompletedDealsQuery>()))
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
            var useCasesDeal = CreateUseCasesCompletedDealDto(completedDeal);
            var completedDealsList = new List<UseCasesCompletedDealDto> { useCasesDeal };
            var mapped = new List<CompletedDealDto>
            {
                new CompletedDealDto(
                    completedDeal.Id.Value,
                    completedDeal.BuyerClientId.Value,
                    completedDeal.SellerClientId.Value,
                    completedDeal.PropertyId.Value,
                    completedDeal.DealDate,
                    completedDeal.DealAmount.Value,
                    completedDeal.DealType.Name,
                    completedDeal.CreatedAt,
                    completedDeal.UpdatedAt)
            };

            var result = Result.Success<IEnumerable<UseCasesCompletedDealDto>>(completedDealsList);
            _mockGetCompletedDealsByClientIdHandler.Setup(x => x.HandleAsync(It.Is<GetCompletedDealsByClientIdQuery>(q => q.ClientId == clientId)))
                .ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompletedDealDto>>(completedDealsList)).Returns(mapped);

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
            Assert.Equal(completedDeal.DealDate, singleItem.DealDate);
            Assert.Equal(completedDeal.DealAmount.Value, singleItem.DealAmount);
            Assert.Equal(completedDeal.DealType.Name, singleItem.DealType);
        }

        [Fact]
        public async Task GetCompletedDealsByClient_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var errorResult = Result.Failure<IEnumerable<UseCasesCompletedDealDto>>("Service error");
            _mockGetCompletedDealsByClientIdHandler.Setup(x => x.HandleAsync(It.Is<GetCompletedDealsByClientIdQuery>(q => q.ClientId == clientId)))
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
            var useCasesDeal = CreateUseCasesCompletedDealDto(completedDeal);
            var completedDealsList = new List<UseCasesCompletedDealDto> { useCasesDeal };
            var mapped = new List<CompletedDealDto>
            {
                new CompletedDealDto(
                    completedDeal.Id.Value,
                    completedDeal.BuyerClientId.Value,
                    completedDeal.SellerClientId.Value,
                    completedDeal.PropertyId.Value,
                    completedDeal.DealDate,
                    completedDeal.DealAmount.Value,
                    completedDeal.DealType.Name,
                    completedDeal.CreatedAt,
                    completedDeal.UpdatedAt)
            };

            var result = Result.Success<IEnumerable<UseCasesCompletedDealDto>>(completedDealsList);
            _mockGetCompletedDealsByPropertyIdHandler.Setup(x => x.HandleAsync(It.Is<GetCompletedDealsByPropertyIdQuery>(q => q.PropertyId == propertyId)))
                .ReturnsAsync(result);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompletedDealDto>>(completedDealsList)).Returns(mapped);

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
            Assert.Equal(completedDeal.DealDate, singleItem.DealDate);
            Assert.Equal(completedDeal.DealAmount.Value, singleItem.DealAmount);
            Assert.Equal(completedDeal.DealType.Name, singleItem.DealType);
        }

        [Fact]
        public async Task GetCompletedDealsByProperty_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var errorResult = Result.Failure<IEnumerable<UseCasesCompletedDealDto>>("Service error");
            _mockGetCompletedDealsByPropertyIdHandler.Setup(x => x.HandleAsync(It.Is<GetCompletedDealsByPropertyIdQuery>(q => q.PropertyId == propertyId)))
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
            _mockDeleteCompletedDealHandler.Setup(x => x.HandleAsync(It.Is<DeleteCompletedDealCommand>(c => c.CompletedDealId == dealId)))
                .ReturnsAsync(result);

            // Act
            var envelope = await _controller.DeleteCompletedDeal(dealId);

            // Assert
            Assert.Equal(204, envelope.Status); // NoContent
            _mockDeleteCompletedDealHandler.Verify(x => x.HandleAsync(It.Is<DeleteCompletedDealCommand>(c => c.CompletedDealId == dealId)), Times.Once);
        }

        [Fact]
        public async Task DeleteCompletedDeal_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure("Completed deal not found");
            _mockDeleteCompletedDealHandler.Setup(x => x.HandleAsync(It.Is<DeleteCompletedDealCommand>(c => c.CompletedDealId == dealId)))
                .ReturnsAsync(errorResult);

            // Act
            var envelope = await _controller.DeleteCompletedDeal(dealId);

            // Assert
            Assert.Equal(404, envelope.Status); // NotFound
            Assert.Contains("Completed deal not found", envelope.Error);
        }
    }
}