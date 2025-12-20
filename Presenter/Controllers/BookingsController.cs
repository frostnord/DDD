using System.Net;
using Domain.Booking;
using Domain.Booking.VO;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.BookingDTO;
using Presenter.Extensions;
using Presenter.Utilities;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;


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
        private readonly IBookingService _bookingService;

        /// <summary>
        /// Конструктор контроллера бронирований
        /// </summary>
        /// <param name="createBookingHandler">Обработчик команды создания бронирования</param>
        /// <param name="confirmBookingHandler">Обработчик команды подтверждения бронирования</param>
        /// <param name="cancelBookingHandler">Обработчик команды отмены бронирования</param>
        /// <param name="bookingRepository">Репозиторий для работы с бронированиями</param>
        /// <param name="bookingService">Сервис</param>
        /// 
        public BookingsController(
            ICommandHandler<CreateBookingCommand, BookingEntity> createBookingHandler,
            ICommandHandler<ConfirmBookingCommand> confirmBookingHandler,
            ICommandHandler<CancelBookingCommand> cancelBookingHandler,
            IBookingRepository bookingRepository,
            IBookingService bookingService)
        {
            _createBookingHandler = createBookingHandler;
            _confirmBookingHandler = confirmBookingHandler;
            _cancelBookingHandler = cancelBookingHandler;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
        }

        /// <summary>
        /// Создание нового бронирования
        /// </summary>
        /// <param name="request">Запрос на создание бронирования с необходимыми данными</param>
        /// <returns>Созданное бронирование с кодом 201 Created</returns>
        [HttpPost]
        public async Task<Envelope> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var command = new CreateBookingCommand(
                request.ClientId,
                request.PropertyId,
                request.AgencyId,
                request.StartDate,
                request.EndDate,
                request.TotalPrice
            );

            var result = await _createBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var bookingDto = result.Value.ToDTO();
            return new Envelope(HttpStatusCode.Created, bookingDto);
        }

        /// <summary>
        /// Получение бронирования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Бронирование с указанным идентификатором</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetBooking(Guid id)
        {
            var bookingIdResult = BookingId.Create(id);
            if (bookingIdResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: "Invalid booking ID");
            }

            var bookingResult = await _bookingRepository.GetByIdAsync(bookingIdResult.Value);
            if (bookingResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: "Booking not found");
            }

            // Проверка на null для дополнительной безопасности
            if (bookingResult.Value == null)
            {
                return new Envelope(HttpStatusCode.NotFound, error: "Booking not found");
            }

            var bookingDto = bookingResult.Value.ToDTO();
            return new Envelope(bookingDto);
        }

        /// <summary>
        /// Получение списка всех бронирований
        /// </summary>
        /// <param name="query">Параметры поиска и фильтрации бронирований</param>
        /// <returns>Список всех бронирований</returns>
        [HttpGet]
        public async Task<Envelope> GetBookings([FromQuery] SearchBookingsQuery query)
        {
            IEnumerable<BookingEntity> booking;
            
            if (query.ClientId.HasValue)
            {
                var bookingResult = await _bookingService.GetByClientIdAsync(query.ClientId.Value);
                booking = bookingResult.Value;
            }
            else if (query.PropertyId.HasValue)
            {
                var bookingResult = await _bookingService.GetByPropertyIdAsync(query.PropertyId.Value);
                booking = bookingResult.Value;
            }
            else
            {
                return new Envelope(HttpStatusCode.BadRequest, error: "Нужен id клиента или недвижимости");
                // var bookingResult = await _bookingService.GetAllBookingsAsync(); 
                // booking = bookingResult.Value;
            }
            
            var bookingDtos = booking.Select(b => b.ToDTO()).ToList();
            return new Envelope(bookingDtos);
        }

        /// <summary>
        /// Подтверждение бронирования
        /// </summary>
        /// <param name="id">Идентификатор бронирования для подтверждения</param>
        /// <returns>Результат подтверждения бронирования</returns>
        [HttpPut("{id}/confirm")]
        public async Task<Envelope> ConfirmBooking(Guid id)
        {
            var command = new ConfirmBookingCommand ( id );
            var result = await _confirmBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(new { Message = "Booking confirmed successfully" });
        }

        /// <summary>
        /// Отмена бронирования
        /// </summary>
        /// <param name="id">Идентификатор бронирования для отмены</param>
        /// <returns>Результат отмены бронирования</returns>
        [HttpPut("{id}/cancel")]
        public async Task<Envelope> CancelBooking(Guid id)
        {
            var command = new CancelBookingCommand ( id );
            var result = await _cancelBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(new { Message = "Booking cancelled successfully" });
        }
    }
}