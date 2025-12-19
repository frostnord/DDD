using Domain.Booking;
using Domain.Booking.VO;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.BookingDTO;
using Presenter.Extensions;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;


namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления бронированиями
    /// Реализует CRUD-операции и бизнес-процессы, связанные с бронированиями недвижимости
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ICommandHandler<CreateBookingCommand, BookingEntity> _createBookingHandler;
        private readonly ICommandHandler<ConfirmBookingCommand> _confirmBookingHandler;
        private readonly ICommandHandler<CancelBookingCommand> _cancelBookingHandler;
        private readonly IBookingRepository _bookingRepository;

        /// <summary>
        /// Конструктор контроллера бронирований
        /// </summary>
        /// <param name="createBookingHandler">Обработчик команды создания бронирования</param>
        /// <param name="confirmBookingHandler">Обработчик команды подтверждения бронирования</param>
        /// <param name="cancelBookingHandler">Обработчик команды отмены бронирования</param>
        /// <param name="bookingRepository">Репозиторий для работы с бронированиями</param>
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

        /// <summary>
        /// Создание нового бронирования
        /// </summary>
        /// <param name="request">Запрос на создание бронирования с необходимыми данными</param>
        /// <returns>Созданное бронирование с кодом 201 Created</returns>
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

        /// <summary>
        /// Получение бронирования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Бронирование с указанным идентификатором</returns>
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

        /// <summary>
        /// Получение списка всех бронирований
        /// </summary>
        /// <param name="query">Параметры поиска и фильтрации бронирований</param>
        /// <returns>Список всех бронирований</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings([FromQuery] SearchBookingsQuery query)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var bookingDtos = bookings.Select(b => b.ToDTO()).ToList();
            return Ok(bookingDtos);
        }

        /// <summary>
        /// Подтверждение бронирования
        /// </summary>
        /// <param name="id">Идентификатор бронирования для подтверждения</param>
        /// <returns>Результат подтверждения бронирования</returns>
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

        /// <summary>
        /// Отмена бронирования
        /// </summary>
        /// <param name="id">Идентификатор бронирования для отмены</param>
        /// <returns>Результат отмены бронирования</returns>
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