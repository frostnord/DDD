using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.BookingDTO;
using Presenter.Utilities;
using UseCases.Booking.Commands;
using UseCases.Booking.Queries.GetBookingById;
using UseCases.Booking.Queries.SearchBookingsQuery;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCasesBookingDto = UseCases.UseCases.DTO.Booking.BookingDto;
using PresenterSearchBookingsQuery = Presenter.DTOs.BookingDTO.SearchBookingsQuery;
using UseCasesSearchBookingsQuery = UseCases.Booking.Queries.SearchBookingsQuery.SearchBookingsQuery;
using Xunit;

namespace Test.Controllers
{
    public class BookingsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateBookingCommand, Guid>> _mockCreateBookingHandler;
        private readonly Mock<ICommandHandler<ConfirmBookingCommand>> _mockConfirmBookingHandler;
        private readonly Mock<ICommandHandler<CancelBookingCommand>> _mockCancelBookingHandler;
        private readonly Mock<IQueryHandler<GetBookingByIdQuery, Result<UseCasesBookingDto>>> _mockGetBookingByIdHandler;
        private readonly Mock<IQueryHandler<UseCasesSearchBookingsQuery, Result<SearchBookingsQueryResponse>>> _mockSearchBookingsHandler;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ReservationsController _controller;

        public BookingsControllerTests()
        {
            _mockCreateBookingHandler = new Mock<ICommandHandler<CreateBookingCommand, Guid>>();
            _mockConfirmBookingHandler = new Mock<ICommandHandler<ConfirmBookingCommand>>();
            _mockCancelBookingHandler = new Mock<ICommandHandler<CancelBookingCommand>>();
            _mockGetBookingByIdHandler = new Mock<IQueryHandler<GetBookingByIdQuery, Result<UseCasesBookingDto>>>();
            _mockSearchBookingsHandler = new Mock<IQueryHandler<UseCasesSearchBookingsQuery, Result<SearchBookingsQueryResponse>>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ReservationsController(
                _mockCreateBookingHandler.Object,
                _mockConfirmBookingHandler.Object,
                _mockCancelBookingHandler.Object,
                _mockGetBookingByIdHandler.Object,
                _mockSearchBookingsHandler.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task CreateBooking_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid()
            };

            var command = new CreateBookingCommand(
                request.ClientId,
                request.PropertyId);

            _mockMapper.Setup(m => m.Map<CreateBookingCommand>(request)).Returns(command);

            var bookingId = Guid.NewGuid();
            var result = Result.Success(bookingId);
            _mockCreateBookingHandler.Setup(x => x.HandleAsync(command))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateBooking(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(201, envelope.Status);
            var createdId = Assert.IsType<Guid>(envelope.Result);
            Assert.Equal(bookingId, createdId);
        }

        [Fact]
        public async Task CreateBooking_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid()
            };

            var command = new CreateBookingCommand(
                request.ClientId,
                request.PropertyId);

            _mockMapper.Setup(m => m.Map<CreateBookingCommand>(request)).Returns(command);

            var errorResult = Result.Failure<Guid>("Validation error");
            _mockCreateBookingHandler.Setup(x => x.HandleAsync(command))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateBooking(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.NotNull(envelope.Error);
            Assert.Contains("Validation error", envelope.Error!.ToString());
        }

        [Fact]
        public async Task GetBooking_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var useCasesDto = new UseCasesBookingDto(
                bookingId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(5),
                "Active",
                DateTime.UtcNow,
                null);

            var presenterDto = new BookingDto(
                useCasesDto.Id,
                useCasesDto.ClientId,
                useCasesDto.PropertyId,
                useCasesDto.ReservedAt,
                useCasesDto.ReservedUntil,
                useCasesDto.Status,
                useCasesDto.CreatedAt,
                useCasesDto.UpdatedAt);

            _mockGetBookingByIdHandler
                .Setup(x => x.HandleAsync(It.Is<GetBookingByIdQuery>(q => q.BookingId == bookingId)))
                .ReturnsAsync(Result.Success(useCasesDto));
            _mockMapper.Setup(m => m.Map<BookingDto>(useCasesDto)).Returns(presenterDto);

            // Act
            var actionResult = await _controller.GetBooking(bookingId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var bookingDto = Assert.IsType<BookingDto>(envelope.Result);
            Assert.Equal(bookingId, bookingDto.Id);
        }

        [Fact]
        public async Task GetBooking_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var actionResult = await _controller.GetBooking(invalidId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.NotNull(envelope.Error);
            Assert.Contains("Invalid booking ID", envelope.Error!.ToString());
        }

        [Fact]
        public async Task GetBooking_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var errorResult = Result.Failure<UseCasesBookingDto>("Booking not found");
            _mockGetBookingByIdHandler
                .Setup(x => x.HandleAsync(It.Is<GetBookingByIdQuery>(q => q.BookingId == bookingId)))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetBooking(bookingId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(404, envelope.Status);
            Assert.NotNull(envelope.Error);
            Assert.Contains("Booking not found", envelope.Error!.ToString());
        }

        [Fact]
        public async Task GetBookings_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var useCasesDtos = new List<UseCasesBookingDto>
            {
                new UseCasesBookingDto(
                    Guid.NewGuid(),
                    clientId,
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(5),
                    "Active",
                    DateTime.UtcNow,
                    null)
            };

            var presenterDtos = useCasesDtos
                .Select(d => new BookingDto(d.Id, d.ClientId, d.PropertyId, d.ReservedAt, d.ReservedUntil, d.Status, d.CreatedAt, d.UpdatedAt))
                .ToList();

            _mockSearchBookingsHandler
                .Setup(x => x.HandleAsync(It.Is<UseCasesSearchBookingsQuery>(q => q.ClientId == clientId && q.PropertyId == null)))
                .ReturnsAsync(Result.Success(new SearchBookingsQueryResponse(useCasesDtos)));

            _mockMapper
                .Setup(m => m.Map<IEnumerable<BookingDto>>(useCasesDtos))
                .Returns(presenterDtos);

            // Act
            var actionResult = await _controller.GetBookings(new PresenterSearchBookingsQuery(clientId, null));

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var response = Assert.IsType<BookingsResponse>(envelope.Result);
            Assert.Single(response.Items);
        }

        [Fact]
        public async Task ConfirmBooking_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var result = Result.Success();
            _mockConfirmBookingHandler.Setup(x => x.HandleAsync(It.IsAny<ConfirmBookingCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.ConfirmBooking(propertyId, clientId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(204, envelope.Status);
        }

        [Fact]
        public async Task ConfirmBooking_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockConfirmBookingHandler.Setup(x => x.HandleAsync(It.IsAny<ConfirmBookingCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.ConfirmBooking(propertyId, clientId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.NotNull(envelope.Error);
            Assert.Contains("Validation error", envelope.Error!.ToString());
        }

        [Fact]
        public async Task CancelBooking_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var result = Result.Success();
            _mockCancelBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CancelBookingCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CancelBooking(propertyId, clientId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(204, envelope.Status);
        }

        [Fact]
        public async Task CancelBooking_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockCancelBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CancelBookingCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CancelBooking(propertyId, clientId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.NotNull(envelope.Error);
            Assert.Contains("Validation error", envelope.Error!.ToString());
        }
    }
}