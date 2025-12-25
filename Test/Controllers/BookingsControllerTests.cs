using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs.BookingDTO;
using Presenter.Utilities;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;
using Xunit;

namespace Test.Controllers
{
    public class BookingsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateBookingCommand, Guid>> _mockCreateBookingHandler;
        private readonly Mock<ICommandHandler<ConfirmBookingCommand>> _mockConfirmBookingHandler;
        private readonly Mock<ICommandHandler<CancelBookingCommand>> _mockCancelBookingHandler;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            _mockCreateBookingHandler = new Mock<ICommandHandler<CreateBookingCommand, Guid>>();
            _mockConfirmBookingHandler = new Mock<ICommandHandler<ConfirmBookingCommand>>();
            _mockCancelBookingHandler = new Mock<ICommandHandler<CancelBookingCommand>>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockBookingService = new Mock<IBookingService>();
            _controller = new BookingsController(
                _mockCreateBookingHandler.Object,
                _mockConfirmBookingHandler.Object,
                _mockCancelBookingHandler.Object,
                _mockBookingService.Object);
        }

        [Fact]
        public async Task CreateBooking_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ClientId = Guid.NewGuid(),
                PropertyId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalPrice = 100
            };

            var booking = BookingEntity.Create(
                ClientId.Create(request.ClientId).Value,
                PropertyId.Create(request.PropertyId).Value,
                Period.Create(request.StartDate, request.EndDate).Value,
                Price.Create(request.TotalPrice).Value
            ).Value;

            var bookingId = booking.Id.Value;
            var result = Result.Success(bookingId);
            _mockCreateBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CreateBookingCommand>()))
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
                PropertyId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalPrice = 100
            };

            var errorResult = Result.Failure<Guid>("Validation error");
            _mockCreateBookingHandler.Setup(x => x.HandleAsync(It.IsAny<CreateBookingCommand>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateBooking(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Validation error", envelope.Error.ToString());
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
                Period.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3)).Value,
                Price.Create(1000).Value
            });

            var result = Result.Success(booking);
            _mockBookingService.Setup(x => x.GetBookingByIdAsync(bookingId))
                .ReturnsAsync(result);

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
            Assert.Contains("Invalid booking ID", envelope.Error.ToString());
        }

        [Fact]
        public async Task GetBooking_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var bookingVOId = BookingId.Create(bookingId).Value;

            var errorResult = Result.Failure<BookingEntity>("Booking not found");
            _mockBookingService.Setup(x => x.GetBookingByIdAsync(bookingId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetBooking(bookingId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(404, envelope.Status);
            Assert.Contains("Booking not found", envelope.Error.ToString());
        }

        [Fact]
        public async Task GetBookings_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookings = new List<BookingEntity>
            {
                BookingEntity.Create(
                    ClientId.Create(clientId).Value,
                    PropertyId.Create(Guid.NewGuid()).Value,
                    Period.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3)).Value,
                    Price.Create(10).Value
                ).Value
            };

            _mockBookingService
                .Setup(x => x.GetByClientIdAsync(clientId))
                .ReturnsAsync(Result.Success<IEnumerable<BookingEntity>>(bookings));

            // Act
            var actionResult = await _controller.GetBookings(new SearchBookingsQuery(clientId, null));

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var responseObj = envelope.Result;
            Assert.NotNull(responseObj);

            IEnumerable<BookingDto> items;
            var itemsProperty = responseObj.GetType().GetProperty("Items", BindingFlags.Instance | BindingFlags.Public);
            if (itemsProperty != null)
            {
                var value = itemsProperty.GetValue(responseObj);
                items = Assert.IsAssignableFrom<IEnumerable<BookingDto>>(value);
            }
            else
            {
                items = Assert.IsAssignableFrom<IEnumerable<BookingDto>>(responseObj);
            }

            Assert.Single(items);
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(200, envelope.Status);
            Assert.Contains("Booking confirmed successfully", envelope.Result.ToString());
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Validation error", envelope.Error.ToString());
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(200, envelope.Status);
            Assert.Contains("Booking cancelled successfully", envelope.Result.ToString());
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
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Validation error", envelope.Error.ToString());
        }
    }
}