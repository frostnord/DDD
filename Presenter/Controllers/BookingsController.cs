using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
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
        private readonly ICommandHandler<CreateBookingCommand, Guid> _createBookingHandler;
        private readonly ICommandHandler<ConfirmBookingCommand> _confirmBookingHandler;
        private readonly ICommandHandler<CancelBookingCommand> _cancelBookingHandler;
        private readonly IBookingService _bookingService;

        /// <summary>
        /// Конструктор контроллера бронирований
        /// </summary>
        /// <param name="createBookingHandler">Обработчик команды создания бронирования</param>
        /// <param name="confirmBookingHandler">Обработчик команды подтверждения бронирования</param>
        /// <param name="cancelBookingHandler">Обработчик команды отмены бронирования</param>
        /// <param name="bookingService">Сервис</param>
        /// 
        public BookingsController(
            ICommandHandler<CreateBookingCommand, Guid> createBookingHandler,
            ICommandHandler<ConfirmBookingCommand> confirmBookingHandler,
            ICommandHandler<CancelBookingCommand> cancelBookingHandler,
            IBookingService bookingService)
        {
            _createBookingHandler = createBookingHandler;
            _confirmBookingHandler = confirmBookingHandler;
            _cancelBookingHandler = cancelBookingHandler;
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
                request.StartDate,
                request.EndDate,
                request.TotalPrice
            );

            var result = await _createBookingHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var bookingDto = result.Value;
            return new Envelope(HttpStatusCode.Created, bookingDto);
        }

        /// <summary>
        /// Возвращает бронирование по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Информация о бронировании</returns>
        /// <response code="200">Возвращает информацию о бронировании</response>
        /// <response code="400">Если идентификатор бронирования некорректен</response>
        /// <response code="404">Если бронирование не найдено</response>
        [HttpGet("{id}")]
        public async Task<Envelope> GetBooking(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: "Invalid booking ID");
            }

            var bookingResult = await _bookingService.GetBookingByIdAsync(id);
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
        /// Возвращает список бронирований с возможностью фильтрации по клиенту или недвижимости
        /// </summary>
        /// <param name="query">Параметры поиска бронирований</param>
        /// <returns>Список бронирований</returns>
        /// <response code="200">Возвращает список бронирований</response>
        /// <response code="400">Если параметры запроса некорректны</response>
        [HttpGet]
        public async Task<Envelope> GetBookings([FromQuery] SearchBookingsQuery query)
        {
            IEnumerable<BookingEntity> booking;

            // Если заданы фильтры по клиенту или недвижимости, используем соответствующие методы сервиса
            if (query.ClientId.HasValue)
            {
                var bookingResult = await _bookingService.GetByClientIdAsync(query.ClientId.Value);
                if (bookingResult.IsFailure)
                {
                    return new Envelope(HttpStatusCode.BadRequest, error: bookingResult.Error);
                }

                booking = bookingResult.Value;
            }
            else if (query.PropertyId.HasValue)
            {
                var bookingResult = await _bookingService.GetByPropertyIdAsync(query.PropertyId.Value);
                if (bookingResult.IsFailure)
                {
                    return new Envelope(HttpStatusCode.BadRequest, error: bookingResult.Error);
                }

                booking = bookingResult.Value;
            }
            else
            {
                return new Envelope(HttpStatusCode.BadRequest, error: "Нужен id клиента или недвижимости");
            }

            var bookingDtos = booking.Select(b => b.ToDTO()).ToList();

            return new Envelope(new
            {
                Items = bookingDtos,
            });
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