using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.BookingDTO;
using Presenter.Utilities;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Booking.Queries.GetBookingById;
using UseCases.Booking.Queries.SearchBookingsQuery;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using AutoMapper;
using UseCasesBookingDto = UseCases.UseCases.DTO.Booking.BookingDto;
using PresenterSearchBookingsQuery = Presenter.DTOs.BookingDTO.SearchBookingsQuery;
using UseCasesSearchBookingsQuery = UseCases.Booking.Queries.SearchBookingsQuery.SearchBookingsQuery;


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
        private readonly IQueryHandler<GetBookingByIdQuery, Result<UseCasesBookingDto>> _getBookingByIdHandler;
        private readonly IQueryHandler<UseCasesSearchBookingsQuery, Result<SearchBookingsQueryResponse>> _searchBookingsHandler;
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор контроллера бронирований
        /// </summary>
        /// <param name="createBookingHandler">Обработчик команды создания бронирования</param>
        /// <param name="confirmBookingHandler">Обработчик команды подтверждения бронирования</param>
        /// <param name="cancelBookingHandler">Обработчик команды отмены бронирования</param>
        /// <param name="getBookingByIdHandler">Обработчик запроса получения бронирования по идентификатору</param>
        /// <param name="searchBookingsHandler">Обработчик запроса поиска бронирований</param>
        /// <param name="mapper">AutoMapper</param>
        public BookingsController(
            ICommandHandler<CreateBookingCommand, Guid> createBookingHandler,
            ICommandHandler<ConfirmBookingCommand> confirmBookingHandler,
            ICommandHandler<CancelBookingCommand> cancelBookingHandler,
            IQueryHandler<GetBookingByIdQuery, Result<UseCasesBookingDto>> getBookingByIdHandler,
            IQueryHandler<UseCasesSearchBookingsQuery, Result<SearchBookingsQueryResponse>> searchBookingsHandler,
            IMapper mapper)
        {
            _createBookingHandler = createBookingHandler;
            _confirmBookingHandler = confirmBookingHandler;
            _cancelBookingHandler = cancelBookingHandler;
            _getBookingByIdHandler = getBookingByIdHandler;
            _searchBookingsHandler = searchBookingsHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Создание нового бронирования
        /// </summary>
        /// <param name="request">Запрос на создание бронирования с необходимыми данными</param>
        /// <returns>Созданное бронирование с кодом 201 Created</returns>
        [HttpPost]
        public async Task<Envelope> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var command = _mapper.Map<CreateBookingCommand>(request);

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
            var query = new GetBookingByIdQuery(id);
            var bookingResult = await _getBookingByIdHandler.HandleAsync(query);
            if (bookingResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: bookingResult.Error);
            }

            var response = _mapper.Map<BookingDto>(bookingResult.Value);
            return new Envelope(response);
        }

        /// <summary>
        /// Возвращает список бронирований с возможностью фильтрации по клиенту или недвижимости
        /// </summary>
        /// <param name="query">Параметры поиска бронирований</param>
        /// <returns>Список бронирований</returns>
        /// <response code="200">Возвращает список бронирований</response>
        /// <response code="400">Если параметры запроса некорректны</response>
        [HttpGet]
        public async Task<Envelope> GetBookings([FromQuery] PresenterSearchBookingsQuery query)
        {
            var useCasesQuery = new UseCasesSearchBookingsQuery(query.ClientId, query.PropertyId);
            var result = await _searchBookingsHandler.HandleAsync(useCasesQuery);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var items = _mapper.Map<IEnumerable<BookingDto>>(result.Value.Items);
            return new Envelope(new BookingsResponse(items));
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

            return new Envelope(HttpStatusCode.NoContent);
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

            return new Envelope(HttpStatusCode.NoContent);
        }
    }
}