using Domain.Booking;
using Domain.Booking.VO;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ICommandHandler<CreateBookingCommand, BookingEntity> _createBookingHandler;
        private readonly ICommandHandler<ConfirmBookingCommand> _confirmBookingHandler;
        private readonly ICommandHandler<CancelBookingCommand> _cancelBookingHandler;
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(
            ICommandHandler<CreateBookingCommand, BookingEntity> createBookingHandler,
            ICommandHandler<ConfirmBookingCommand> confirmBookingHandler,
            ICommandHandler<CancelBookingCommand> cancelBookingHandler,
            IBookingRepository bookingRepository)
        {
            _createBookingHandler = createBookingHandler;
            _confirmBookingHandler = confirmBookingHandler;
            _cancelBookingHandler = cancelBookingHandler;
            _bookingRepository = bookingRepository;
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var command = new CreateBookingCommand
            {
                ClientId = request.ClientId,
                PropertyId = request.PropertyId,
                AgencyId = request.AgencyId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPrice = request.TotalPrice
            };

            var result = await _createBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
        {
            var bookingIdResult = BookingId.Create(id);
            if (bookingIdResult.IsFailure)
            {
                return BadRequest(new { Error = "Invalid booking ID" });
            }

            var bookingResult = await _bookingRepository.GetByIdAsync(bookingIdResult.Value);
            if (bookingResult.IsFailure)
            {
                return NotFound(new { Error = "Booking not found" });
            }

            // Проверка на null для дополнительной безопасности
            if (bookingResult.Value == null)
            {
                return NotFound(new { Error = "Booking not found" });
            }

            var bookingDto = bookingResult.Value.ToDTO();
            return Ok(bookingDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings([FromQuery] SearchBookingsQuery query)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var bookingDtos = bookings.Select(b => b.ToDTO()).ToList();
            return Ok(bookingDtos);
        }

        [HttpPut("{id}/confirm")]
        public async Task<ActionResult> ConfirmBooking(Guid id)
        {
            var command = new ConfirmBookingCommand { BookingId = id };
            var result = await _confirmBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(new { Message = "Booking confirmed successfully" });
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelBooking(Guid id)
        {
            var command = new CancelBookingCommand { BookingId = id };
            var result = await _cancelBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(new { Message = "Booking cancelled successfully" });
        }
    }
}