using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.BuyerDTO;
using Presenter.Utilities;

using UseCases.Buyer;
using UseCases.Buyer.Commands.CreateBuyer;
using UseCases.Buyer.Commands.DeleteBuyer;
using UseCases.Buyer.Commands.UpdateBuyer;
using UseCases.Buyer.Queries.GetBuyerById;
using UseCases.Buyer.Queries.SearchBuyersQuery;
using UseCases.UseCases.DTO.Buyer;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using Xunit;

namespace Test.Controllers
{
    public class BuyersControllerTests
    {
        private readonly Mock<ICommandHandler<CreateBuyerCommand, Guid>> _mockCreateBuyerHandler;
        private readonly Mock<ICommandHandler<UpdateBuyerCommand>> _mockUpdateBuyerHandler;
        private readonly Mock<ICommandHandler<DeleteBuyerCommand>> _mockDeleteBuyerHandler;
        private readonly Mock<IQueryHandler<GetBuyerByIdQuery, Result<BuyerDto>>> _mockGetBuyerByIdHandler;
        private readonly Mock<IQueryHandler<SearchBuyersQuery, Result<SearchBuyersQueryResponse>>> _mockSearchBuyersHandler;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BuyersController _controller;

        public BuyersControllerTests()
        {
            _mockCreateBuyerHandler = new Mock<ICommandHandler<CreateBuyerCommand, Guid>>();
            _mockUpdateBuyerHandler = new Mock<ICommandHandler<UpdateBuyerCommand>>();
            _mockDeleteBuyerHandler = new Mock<ICommandHandler<DeleteBuyerCommand>>();
            _mockGetBuyerByIdHandler = new Mock<IQueryHandler<GetBuyerByIdQuery, Result<BuyerDto>>>();
            _mockSearchBuyersHandler = new Mock<IQueryHandler<SearchBuyersQuery, Result<SearchBuyersQueryResponse>>>();
            _mockMapper = new Mock<IMapper>();

            _controller = new BuyersController(
                _mockCreateBuyerHandler.Object,
                _mockUpdateBuyerHandler.Object,
                _mockDeleteBuyerHandler.Object,
                _mockGetBuyerByIdHandler.Object,
                _mockSearchBuyersHandler.Object,
                _mockMapper.Object
            );
        }

        private BuyerDto CreateTestBuyerDto(Guid buyerId)
        {
            return new BuyerDto(buyerId, Guid.NewGuid(), DateTime.UtcNow);
        }

        private CreateBuyerRequest CreateValidCreateRequest()
        {
            return new CreateBuyerRequest
            {
                ClientId = Guid.NewGuid(),
                PreferredNumberOfRooms = 2,
                PreferredFloor = 3,
                PreferredTotalFloors = 9,
                PreferredType = "Apartment",
                PreferParking = true,
                PreferredHeatingType = "Central",
                PreferredCondition = "Good"
            };
        }

        private UpdateBuyerRequest CreateValidUpdateRequest()
        {
            return new UpdateBuyerRequest
            {
                ClientId = Guid.NewGuid(),
                PreferredNumberOfRooms = 3,
                PreferredFloor = 4,
                PreferredTotalFloors = 10,
                PreferredType = "House",
                PreferParking = false,
                PreferredHeatingType = "Autonomous",
                PreferredCondition = "Excellent"
            };
        }

        [Fact]
        public async Task CreateBuyer_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var command = new CreateBuyerCommand(
                request.ClientId,
                request.PreferredNumberOfRooms,
                request.PreferredFloor,
                request.PreferredTotalFloors,
                request.PreferredType,
                request.PreferParking,
                request.PreferredHeatingType,
                request.PreferredCondition);
            var newBuyerId = Guid.NewGuid();

            _mockMapper.Setup(m => m.Map<CreateBuyerCommand>(request)).Returns(command);
            _mockCreateBuyerHandler
                .Setup(h => h.HandleAsync(It.Is<CreateBuyerCommand>(c => c == command)))
                .ReturnsAsync(Result.Success(newBuyerId));

            // Act
            var result = await _controller.CreateBuyer(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.Created, envelope.Status);
            Assert.Equal(newBuyerId, envelope.Result);
        }

        [Fact]
        public async Task CreateBuyer_HandlerFailure_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var command = new CreateBuyerCommand(
                request.ClientId,
                request.PreferredNumberOfRooms,
                request.PreferredFloor,
                request.PreferredTotalFloors,
                request.PreferredType,
                request.PreferParking,
                request.PreferredHeatingType,
                request.PreferredCondition);
            var error = "Handler failure";

            _mockMapper.Setup(m => m.Map<CreateBuyerCommand>(request)).Returns(command);
            _mockCreateBuyerHandler
                .Setup(h => h.HandleAsync(It.Is<CreateBuyerCommand>(c => c == command)))
                .ReturnsAsync(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.CreateBuyer(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }

        [Fact]
        public async Task GetBuyer_ExistingId_ReturnsOk()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var buyerDto = CreateTestBuyerDto(buyerId);

            _mockGetBuyerByIdHandler.Setup(h => h.HandleAsync(It.Is<GetBuyerByIdQuery>(q => q.BuyerId == buyerId)))
                .ReturnsAsync(Result.Success(buyerDto));

            // Act
            var result = await _controller.GetBuyer(buyerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.OK, envelope.Status);
            Assert.Equal(buyerDto, envelope.Result);
        }

        [Fact]
        public async Task GetBuyer_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var error = "Buyer not found";

            _mockGetBuyerByIdHandler.Setup(h => h.HandleAsync(It.Is<GetBuyerByIdQuery>(q => q.BuyerId == buyerId)))
                .ReturnsAsync(Result.Failure<BuyerDto>(error));

            // Act
            var result = await _controller.GetBuyer(buyerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }

        [Fact]
        public async Task UpdateBuyer_ValidRequest_ReturnsOk()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var request = CreateValidUpdateRequest();
            var command = new UpdateBuyerCommand(
                buyerId,
                request.ClientId,
                request.PreferredNumberOfRooms,
                request.PreferredFloor,
                request.PreferredTotalFloors,
                request.PreferredType,
                request.PreferParking,
                request.PreferredHeatingType,
                request.PreferredCondition);

            _mockMapper
                .Setup(m => m.Map<UpdateBuyerCommand>(request, It.IsAny<Action<IMappingOperationOptions>>()))
                .Returns(command);
            _mockUpdateBuyerHandler.Setup(h => h.HandleAsync(command)).ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.UpdateBuyer(buyerId, request);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, envelope.Status);
        }

        [Fact]
        public async Task GetBuyers_ValidQuery_ReturnsOk()
        {
            // Arrange
            var query = new SearchBuyersQuery();
            var buyerDto = CreateTestBuyerDto(Guid.NewGuid());
            var searchResult = new SearchBuyersQueryResponse(new List<BuyerDto> { buyerDto }, 1, 10, 1);

            _mockSearchBuyersHandler.Setup(h => h.HandleAsync(query)).ReturnsAsync(Result.Success(searchResult));

            // Act
            var result = await _controller.GetBuyers(query);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.OK, envelope.Status);
            var pagedResponse = Assert.IsType<PagedBuyersResponse>(envelope.Result);
            Assert.Single(pagedResponse.Items);
        }

        [Fact]
        public async Task DeleteBuyer_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var command = new DeleteBuyerCommand(buyerId);

            _mockDeleteBuyerHandler.Setup(h => h.HandleAsync(command)).ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.DeleteBuyer(buyerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, envelope.Status);
        }

        [Fact]
        public async Task DeleteBuyer_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var command = new DeleteBuyerCommand(buyerId);
            var error = "Buyer not found";

            _mockDeleteBuyerHandler.Setup(h => h.HandleAsync(command)).ReturnsAsync(Result.Failure(error));

            // Act
            var result = await _controller.DeleteBuyer(buyerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }
    }
}