using System.Reflection;
using CSharpFunctionalExtensions;
using Domain.Agency.VO;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;
using Xunit;

namespace Test.Controllers
{
    public class BookingsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateBookingCommand, BookingEntity>> _mockCreateBookingHandler;
        private readonly Mock<ICommandHandler<ConfirmBookingCommand>> _mockConfirmBookingHandler;
        private readonly Mock<ICommandHandler<CancelBookingCommand>> _mockCancelBookingHandler;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            _mockCreateBookingHandler = new Mock<ICommandHandler<CreateBookingCommand, BookingEntity>>();
            _mockConfirmBookingHandler = new Mock<ICommandHandler<ConfirmBookingCommand>>();
            _mockCancelBookingHandler = new Mock<ICommandHandler<CancelBookingCommand>>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _controller = new BookingsController(
                _mockCreateBookingHandler.Object,
                _mockConfirmBookingHandler.Object,
                _mockCancelBookingHandler.Object,
                _mockBookingRepository.Object);
        }

        [Fact]
        public async Task CreateBooking_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                AgencyId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalPrice = 100
            };

            var booking = BookingEntity.Create(
                ClientId.Create(request.ClientId).Value,
                PropertyId.Create(request.PropertyId).Value,
                AgencyId.Create(request.AgencyId).Value,
                Period.Create(request.StartDate, request.EndDate).Value,
                Price.Create(request.TotalPrice).Value
            ).Value;

            var result = Result.Success(booking);
            _mockCreateBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CreateBookingCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateBooking(request);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetBooking", createdAtResult.ActionName);
            Assert.Equal(booking.Id.Value, createdAtResult.RouteValues["id"]);
        }

        [Fact]
        public async Task CreateBooking_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                AgencyId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalPrice = 100
            };

            var errorResult = Result.Failure<BookingEntity>("Validation error");
            _mockCreateBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CreateBookingCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateBooking(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Contains("Validation error", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetBooking_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var bookingVOId = BookingId.Create(bookingId).Value;

            // Создаем бронирование с нужным ID
            var bookingConstructor =
                typeof(BookingEntity).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();
            var booking = (BookingEntity)bookingConstructor.Invoke(new object[]
            {
                bookingVOId,
                ClientId.Create(Guid.NewGuid()).Value,
                PropertyId.Create(Guid.NewGuid()).Value,
                AgencyId.Create(Guid.NewGuid()).Value,
                Period.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3)).Value,
                Price.Create(1000).Value
            });

            var result = Result.Success(booking);
            _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingVOId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var bookingDto = Assert.IsType<BookingDto>(okResult.Value);
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
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Contains("Invalid booking ID", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetBooking_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var bookingVOId = BookingId.Create(bookingId).Value;

            var errorResult = Result.Failure<BookingEntity>("Booking not found");
            _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingVOId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetBooking(bookingId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Contains("Booking not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetBookings_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var bookings = new List<BookingEntity>
            {
                BookingEntity.Create(
                    ClientId.Create(Guid.NewGuid()).Value,
                    PropertyId.Create(Guid.NewGuid()).Value,
                    AgencyId.Create(Guid.NewGuid()).Value,
                    Period.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3)).Value,
                    Price.Create(10).Value
                ).Value
            };

            _mockBookingRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(bookings);

            // Act
            var actionResult = await _controller.GetBookings(new SearchBookingsQuery());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var bookingsDto = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Single(bookingsDto);
        }

        [Fact]
        public async Task ConfirmBooking_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();

            var result = Result.Success();
            _mockConfirmBookingHandler.Setup(x => x.HandleAsync(It.IsAny<ConfirmBookingCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.ConfirmBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Contains("Booking confirmed successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task ConfirmBooking_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var bookingId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockConfirmBookingHandler.Setup(x => x.HandleAsync(It.IsAny<ConfirmBookingCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.ConfirmBooking(bookingId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Contains("Validation error", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CancelBooking_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();

            var result = Result.Success();
            _mockCancelBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CancelBookingCommand>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CancelBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Contains("Booking cancelled successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task CancelBooking_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var bookingId = Guid.NewGuid();

            var errorResult = Result.Failure("Validation error");
            _mockCancelBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CancelBookingCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CancelBooking(bookingId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Contains("Validation error", badRequestResult.Value.ToString());
        }
    }
}