using System;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.SellerDTO;
using Presenter.Utilities;
using UseCases.DTO.Seller;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Seller.Commands;
using UseCases.Seller.Queries;
using Xunit;

namespace Test.Controllers
{
    public class SellersControllerTests
    {
        private readonly Mock<ICommandHandler<CreateSellerCommand, Guid>> _mockCreateSellerHandler;
        private readonly Mock<ICommandHandler<UpdateSellerCommand>> _mockUpdateSellerHandler;
        private readonly Mock<ICommandHandler<DeleteSellerCommand>> _mockDeleteSellerHandler;
        private readonly Mock<IQueryHandler<GetSellerByIdQuery, Result<SellerDto>>> _mockGetSellerByIdHandler;
        private readonly Mock<IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>>> _mockSearchSellersHandler;
        private readonly SellersController _controller;

        public SellersControllerTests()
        {
            _mockCreateSellerHandler = new Mock<ICommandHandler<CreateSellerCommand, Guid>>();
            _mockUpdateSellerHandler = new Mock<ICommandHandler<UpdateSellerCommand>>();
            _mockDeleteSellerHandler = new Mock<ICommandHandler<DeleteSellerCommand>>();
            _mockGetSellerByIdHandler = new Mock<IQueryHandler<GetSellerByIdQuery, Result<SellerDto>>>();
            _mockSearchSellersHandler = new Mock<IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>>>();

            _controller = new SellersController(
                _mockCreateSellerHandler.Object,
                _mockUpdateSellerHandler.Object,
                _mockDeleteSellerHandler.Object,
                _mockGetSellerByIdHandler.Object,
                _mockSearchSellersHandler.Object
            );
        }

        [Fact]
        public async Task CreateSeller_ValidRequest_ReturnsCreatedResultWithGuid()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var request = new CreateSellerRequest { ClientId = clientId };
            var newSellerId = Guid.NewGuid();

            _mockCreateSellerHandler
                .Setup(h => h.HandleAsync(It.IsAny<CreateSellerCommand>()))
                .ReturnsAsync(Result.Success(newSellerId));

            // Act
            var result = await _controller.CreateSeller(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.Created, envelope.Status);
            Assert.Equal(newSellerId, envelope.Result);
        }

        [Fact]
        public async Task CreateSeller_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateSellerRequest { ClientId = Guid.Empty };
            var error = "Invalid client ID";

            _mockCreateSellerHandler
                .Setup(h => h.HandleAsync(It.IsAny<CreateSellerCommand>()))
                .ReturnsAsync(Result.Failure<Guid>(error));

            // Act
            var result = await _controller.CreateSeller(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }

        [Fact]
        public async Task GetSeller_ExistingId_ReturnsOkResultWithSellerDto()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var sellerDto = new SellerDto(sellerId, Guid.NewGuid(), DateTime.UtcNow);

            _mockGetSellerByIdHandler
                .Setup(h => h.HandleAsync(It.Is<GetSellerByIdQuery>(q => q.SellerId == sellerId)))
                .ReturnsAsync(Result.Success(sellerDto));

            // Act
            var result = await _controller.GetSeller(sellerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.OK, envelope.Status);
            var returnedDto = Assert.IsType<SellerDto>(envelope.Result);
            Assert.Equal(sellerId, returnedDto.Id);
        }

        [Fact]
        public async Task GetSeller_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var error = "Seller not found";

            _mockGetSellerByIdHandler
                .Setup(h => h.HandleAsync(It.Is<GetSellerByIdQuery>(q => q.SellerId == sellerId)))
                .ReturnsAsync(Result.Failure<SellerDto>(error));

            // Act
            var result = await _controller.GetSeller(sellerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, envelope.Status);
            Assert.Equal(error, envelope.Error);
        }

        [Fact]
        public async Task UpdateSeller_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new UpdateSellerRequest { ClientId = clientId };

            _mockUpdateSellerHandler
                .Setup(h => h.HandleAsync(It.Is<UpdateSellerCommand>(c => c.SellerId == sellerId)))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.UpdateSeller(sellerId, request);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, envelope.Status);
        }

        [Fact]
        public async Task DeleteSeller_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var sellerId = Guid.NewGuid();

            _mockDeleteSellerHandler
                .Setup(h => h.HandleAsync(It.Is<DeleteSellerCommand>(c => c.SellerId == sellerId)))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.DeleteSeller(sellerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, envelope.Status);
        }
    }
}