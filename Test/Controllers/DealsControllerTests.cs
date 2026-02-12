using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.DealDTO;
using Presenter.Utilities;
using UseCases.CompleteDeal;
using UseCases.Deal.Commands;
using UseCases.Deal.Queries.GetDealById;
using UseCases.Deal.Queries.SearchDealsQuery;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCasesDealDto = UseCases.UseCases.DTO.Deal.DealDto;
using Xunit;

namespace Test.Controllers
{
    public class DealsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateDealCommand, Guid>> _mockCreateDealHandler;
        private readonly Mock<ICommandHandler<ConfirmDealCommand>> _mockConfirmDealHandler;
        private readonly Mock<ICommandHandler<CompleteDealCommand>> _mockCompleteDealHandler;
        private readonly Mock<ICommandHandler<CancelDealCommand>> _mockCancelDealHandler;
        private readonly Mock<IQueryHandler<GetDealByIdQuery, Result<UseCasesDealDto>>> _mockGetDealByIdHandler;
        private readonly Mock<IQueryHandler<SearchDealsQuery, Result<SearchDealsQueryResponse>>> _mockSearchDealsHandler;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DealsController _controller;

        public DealsControllerTests()
        {
            _mockCreateDealHandler = new Mock<ICommandHandler<CreateDealCommand, Guid>>();
            _mockConfirmDealHandler = new Mock<ICommandHandler<ConfirmDealCommand>>();
            _mockCompleteDealHandler = new Mock<ICommandHandler<CompleteDealCommand>>();
            _mockCancelDealHandler = new Mock<ICommandHandler<CancelDealCommand>>();
            _mockGetDealByIdHandler = new Mock<IQueryHandler<GetDealByIdQuery, Result<UseCasesDealDto>>>();
            _mockSearchDealsHandler = new Mock<IQueryHandler<SearchDealsQuery, Result<SearchDealsQueryResponse>>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new DealsController(
                _mockCreateDealHandler.Object,
                _mockConfirmDealHandler.Object,
                _mockCompleteDealHandler.Object,
                _mockCancelDealHandler.Object,
                _mockGetDealByIdHandler.Object,
                _mockSearchDealsHandler.Object,
                _mockMapper.Object);
        }

        private static DealEntity CreateDealEntity(Guid? clientId = null, Guid? propertyId = null, DealDetails? details = null)
        {
            var clientIdValue = ClientId.Create(clientId ?? Guid.NewGuid()).Value;
            var propertyIdValue = PropertyId.Create(propertyId ?? Guid.NewGuid()).Value;

            var dealDetails = details ?? DealDetails.Create(
                DateTime.UtcNow,
                Domain.ValueObjects.Price.Create(1000).Value,
                "Test deal",
                string.Empty).Value;

            return DealEntity.Create(clientIdValue, propertyIdValue, dealDetails).Value;
        }

        private static IEnumerable<DealResponse> ExtractItems(dynamic response)
        {
            var itemsProperty = response.GetType().GetProperty("Items", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(itemsProperty);

            var value = itemsProperty.GetValue(response);
            return Assert.IsAssignableFrom<IEnumerable<DealResponse>>(value);
        }

        [Fact]
        public async Task CreateDeal_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateDealRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                Details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(1000).Value, "Test deal", string.Empty).Value
            };

            var command = new CreateDealCommand(request.ClientId, request.PropertyId, request.Details);
            _mockMapper.Setup(m => m.Map<CreateDealCommand>(request)).Returns(command);

            var createdDealId = Guid.NewGuid();
            var result = Result.Success(createdDealId);
            _mockCreateDealHandler.Setup(x => x.HandleAsync(command))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateDeal(request);
            var envelope = Assert.IsType<Envelope>(actionResult);

            // Assert
            Assert.Equal((int)System.Net.HttpStatusCode.Created, envelope.Status);
            Assert.IsType<Guid>(envelope.Result);
            Assert.Equal(createdDealId, (Guid)envelope.Result);
        }

        [Fact]
        public async Task CreateDeal_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateDealRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                Details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(1000).Value, "Test deal", string.Empty).Value
            };

            var command = new CreateDealCommand(request.ClientId, request.PropertyId, request.Details);
            _mockMapper.Setup(m => m.Map<CreateDealCommand>(request)).Returns(command);

            var errorResult = Result.Failure<Guid>("Validation error");
            _mockCreateDealHandler.Setup(x => x.HandleAsync(command))
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
            var clientId = ClientId.Create(Guid.NewGuid()).Value;
            var propertyId = PropertyId.Create(Guid.NewGuid()).Value;
            var details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(100).Value, "Test deal", string.Empty).Value;
            
            var deal = DealEntity.Create(clientId, propertyId, details).Value;

            var useCasesDto = new UseCasesDealDto(
                deal.Id.Value,
                deal.ClientId.Value,
                deal.PropertyId.Value,
                deal.Details,
                deal.Status.Name,
                deal.CreatedAt,
                deal.UpdatedAt);

            var presenterDto = new DealResponse(
                useCasesDto.Id,
                useCasesDto.ClientId,
                useCasesDto.PropertyId,
                useCasesDto.Details,
                useCasesDto.Status,
                useCasesDto.CreatedAt,
                useCasesDto.UpdatedAt);

            _mockGetDealByIdHandler
                .Setup(x => x.HandleAsync(It.Is<GetDealByIdQuery>(q => q.DealId == dealIdGuid)))
                .ReturnsAsync(Result.Success(useCasesDto));
            _mockMapper.Setup(m => m.Map<DealResponse>(useCasesDto)).Returns(presenterDto);

            // Act
            var actionResult = await _controller.GetDeal(dealIdGuid);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var dealDto = Assert.IsType<DealResponse>(envelope.Result);
            Assert.Equal(deal.Id.Value, dealDto.Id);
            Assert.Equal(deal.ClientId.Value, dealDto.ClientId);
            Assert.Equal(deal.PropertyId.Value, dealDto.PropertyId);
            Assert.Equal(deal.Status.Name, dealDto.Status);
        }

        [Fact]
        public async Task GetDeal_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dealId = Guid.NewGuid();

            var errorResult = Result.Failure<UseCasesDealDto>("Deal not found");
            _mockGetDealByIdHandler
                .Setup(x => x.HandleAsync(It.Is<GetDealByIdQuery>(q => q.DealId == dealId)))
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
            var details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(100).Value, "Test deal", string.Empty).Value;
            var clientIdVO = ClientId.Create(clientId).Value;
            var propertyIdVO = PropertyId.Create(Guid.NewGuid()).Value;
            
            var deal = DealEntity.Create(clientIdVO, propertyIdVO, details).Value;
            
            var dealsList = new List<DealEntity> { deal };

            var useCasesDtos = dealsList.ConvertAll(d => new UseCasesDealDto(
                d.Id.Value,
                d.ClientId.Value,
                d.PropertyId.Value,
                d.Details,
                d.Status.Name,
                d.CreatedAt,
                d.UpdatedAt));

            var presenterDtos = useCasesDtos.ConvertAll(d => new DealResponse(
                d.Id,
                d.ClientId,
                d.PropertyId,
                d.Details,
                d.Status,
                d.CreatedAt,
                d.UpdatedAt));

            _mockSearchDealsHandler
                .Setup(x => x.HandleAsync(It.Is<SearchDealsQuery>(q => q.ClientId == clientId && q.PropertyId == null)))
                .ReturnsAsync(Result.Success(new SearchDealsQueryResponse(useCasesDtos, 1, 10, 1)));

            _mockMapper
                .Setup(m => m.Map<IEnumerable<DealResponse>>(useCasesDtos))
                .Returns(presenterDtos);

            // Act
            var actionResult = await _controller.GetDeals(query);
            var envelope = Assert.IsType<Envelope>(actionResult);

            // Assert
            var response = Assert.IsType<PagedDealsResponse>(envelope.Result);
            var singleItem = Assert.Single(response.Items);
            Assert.Equal(deal.Id.Value, singleItem.Id);
            Assert.Equal(deal.ClientId.Value, singleItem.ClientId);
            Assert.Equal(deal.PropertyId.Value, singleItem.PropertyId);
            Assert.Equal(1, response.TotalCount);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(1, response.TotalPages);
            Assert.Equal(1, response.CurrentPage);
        }

        [Fact]
        public async Task GetDeals_WithPropertyId_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var query = new SearchDealsQuery(null, propertyId);
            var details = DealDetails.Create(DateTime.UtcNow, Domain.ValueObjects.Price.Create(100).Value, "Test deal", string.Empty).Value;
            var clientIdVO = ClientId.Create(Guid.NewGuid()).Value;
            var propertyIdVO = PropertyId.Create(propertyId).Value;
            
            var deal = DealEntity.Create(clientIdVO, propertyIdVO, details).Value;
            
            var dealsList = new List<DealEntity> { deal };

            var useCasesDtos = dealsList.ConvertAll(d => new UseCasesDealDto(
                d.Id.Value,
                d.ClientId.Value,
                d.PropertyId.Value,
                d.Details,
                d.Status.Name,
                d.CreatedAt,
                d.UpdatedAt));

            var presenterDtos = useCasesDtos.ConvertAll(d => new DealResponse(
                d.Id,
                d.ClientId,
                d.PropertyId,
                d.Details,
                d.Status,
                d.CreatedAt,
                d.UpdatedAt));

            _mockSearchDealsHandler
                .Setup(x => x.HandleAsync(It.Is<SearchDealsQuery>(q => q.ClientId == null && q.PropertyId == propertyId)))
                .ReturnsAsync(Result.Success(new SearchDealsQueryResponse(useCasesDtos, 1, 10, 1)));

            _mockMapper
                .Setup(m => m.Map<IEnumerable<DealResponse>>(useCasesDtos))
                .Returns(presenterDtos);

            // Act
            var actionResult = await _controller.GetDeals(query);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var response = Assert.IsType<PagedDealsResponse>(envelope.Result);
            var singleItem = Assert.Single(response.Items);
            Assert.Equal(deal.Id.Value, singleItem.Id);
            Assert.Equal(deal.ClientId.Value, singleItem.ClientId);
            Assert.Equal(deal.PropertyId.Value, singleItem.PropertyId);
            Assert.Equal(1, response.TotalCount);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(1, response.TotalPages);
            Assert.Equal(1, response.CurrentPage);
        }

        [Fact]
        public async Task GetDeals_WithoutFilters_ReturnsBadRequest()
        {
            // Arrange
            var query = new SearchDealsQuery(null, null);

            _mockSearchDealsHandler
                .Setup(x => x.HandleAsync(It.Is<SearchDealsQuery>(q => q.ClientId == null && q.PropertyId == null)))
                .ReturnsAsync(Result.Failure<SearchDealsQueryResponse>("Нужен id клиента или недвижимости"));

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
            Assert.Equal((int)System.Net.HttpStatusCode.NoContent, envelope.Status);
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
            Assert.Equal((int)System.Net.HttpStatusCode.NoContent, envelope.Status);
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
            Assert.Equal((int)System.Net.HttpStatusCode.NoContent, envelope.Status);
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