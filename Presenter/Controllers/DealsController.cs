using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.DealDTO;
using Presenter.Utilities;
using UseCases.CompleteDeal;
using UseCases.Deal.Commands;
using UseCases.Deal.Queries.GetDealById;
using UseCases.Deal.Queries.SearchDealsQuery;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCasesDealDto = UseCases.UseCases.DTO.Deal.DealDto;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления сделками в системе недвижимости
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly ICommandHandler<CreateDealCommand, Guid> _createDealHandler;
        private readonly ICommandHandler<ConfirmDealCommand> _confirmDealHandler;
        private readonly ICommandHandler<CompleteDealCommand> _completeDealHandler;
        private readonly ICommandHandler<CancelDealCommand> _cancelDealHandler;
        private readonly IQueryHandler<GetDealByIdQuery, Result<UseCasesDealDto>> _getDealByIdHandler;
        private readonly IQueryHandler<SearchDealsQuery, Result<SearchDealsQueryResponse>> _searchDealsHandler;
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор контроллера сделок
        /// </summary>
        /// <param name="createDealHandler">Обработчик команды создания сделки</param>
        /// <param name="confirmDealHandler">Обработчик команды подтверждения сделки</param>
        /// <param name="completeDealHandler">Обработчик команды завершения сделки</param>
        /// <param name="cancelDealHandler">Обработчик команды отмены сделки</param>
        /// <param name="getDealByIdHandler">Обработчик запроса получения сделки по идентификатору</param>
        /// <param name="searchDealsHandler">Обработчик запроса поиска сделок</param>
        /// <param name="mapper">AutoMapper</param>
        public DealsController(
            ICommandHandler<CreateDealCommand, Guid> createDealHandler,
            ICommandHandler<ConfirmDealCommand> confirmDealHandler,
            ICommandHandler<CompleteDealCommand> completeDealHandler,
            ICommandHandler<CancelDealCommand> cancelDealHandler,
            IQueryHandler<GetDealByIdQuery, Result<UseCasesDealDto>> getDealByIdHandler,
            IQueryHandler<SearchDealsQuery, Result<SearchDealsQueryResponse>> searchDealsHandler,
            IMapper mapper)
        {
            _createDealHandler = createDealHandler;
            _confirmDealHandler = confirmDealHandler;
            _completeDealHandler = completeDealHandler;
            _cancelDealHandler = cancelDealHandler;
            _getDealByIdHandler = getDealByIdHandler;
            _searchDealsHandler = searchDealsHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новую сделку
        /// </summary>
        /// <param name="request">Данные для создания сделки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданная сделка</returns>
        /// <response code="201">Возвращает созданную сделку</response>
        /// <response code="400">Если данные для создания сделки некорректны</response>
        [HttpPost]
        public async Task<Envelope> CreateDeal([FromBody] CreateDealRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateDealCommand>(request);

            var result = await _createDealHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value);
        }

        /// <summary>
        /// Возвращает сделку по её идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Информация о сделке</returns>
        /// <response code="200">Возвращает информацию о сделке</response>
        /// <response code="400">Если идентификатор сделки некорректен</response>
        /// <response code="404">Если сделка не найдена</response>
        [HttpGet("{id}")]
        public async Task<Envelope> GetDeal(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetDealByIdQuery(id);
            var dealResult = await _getDealByIdHandler.HandleAsync(query, cancellationToken);

            if (dealResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: dealResult.Error);
            }

            var response = _mapper.Map<DealResponse>(dealResult.Value);
            return new Envelope(response);
        }

        /// <summary>
        /// Возвращает список сделок с возможностью фильтрации по клиенту или недвижимости
        /// </summary>
        /// <param name="query">Параметры поиска сделок</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список сделок</returns>
        /// <response code="200">Возвращает список сделок</response>
        /// <response code="400">Если параметры запроса некорректны</response>
        
        [HttpGet]
        public async Task<Envelope> GetDeals([FromQuery] SearchDealsQuery query, CancellationToken cancellationToken)
        {
            var result = await _searchDealsHandler.HandleAsync(query, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var searchResult = result.Value;
            var items = _mapper.Map<IEnumerable<DealResponse>>(searchResult.Items).ToList();
            var response = new PagedDealsResponse(
                items,
                searchResult.TotalCount,
                searchResult.PageSize,
                searchResult.TotalPages,
                query.Page);

            return new Envelope(response);
        }

        /// <summary>
        /// Подтверждает сделку
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат подтверждения сделки</returns>
        /// <response code="204">Если сделка успешно подтверждена</response>
        /// <response code="400">Если идентификатор сделки некорректен или произошла ошибка при подтверждении</response>
        [HttpPut("{id}/confirm")]
        public async Task<Envelope> ConfirmDeal(Guid id, CancellationToken cancellationToken)
        {
            var command = new ConfirmDealCommand(id);
            var result = await _confirmDealHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Завершает сделку
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат завершения сделки</returns>
        /// <response code="204">Если сделка успешно завершена</response>
        /// <response code="400">Если идентификатор сделки некорректен или произошла ошибка при завершении</response>
        [HttpPut("{id}/complete")]
        public async Task<Envelope> CompleteDeal(Guid id, CancellationToken cancellationToken)
        {
            var command = new CompleteDealCommand(id);
            var result = await _completeDealHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Отменяет сделку
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат отмены сделки</returns>
        /// <response code="204">Если сделка успешно отменена</response>
        /// <response code="400">Если идентификатор сделки некорректен или произошла ошибка при отмене</response>
        [HttpPut("{id}/cancel")]
        public async Task<Envelope> CancelDeal(Guid id, CancellationToken cancellationToken)
        {
            var command = new CancelDealCommand(id);
            var result = await _cancelDealHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }
    }
}