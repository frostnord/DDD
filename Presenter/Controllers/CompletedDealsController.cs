using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.CompletedDealDTO;
using Presenter.Utilities;
using UseCases.CompleteDeal;
using UseCases.CompleteDeal.Commands.CreateCompliteDealCommand;
using UseCases.CompleteDeal.Commands.DeleteCompletedDeal;
using UseCases.CompleteDeal.Queries.GetAllCompletedDeals;
using UseCases.CompleteDeal.Queries.GetCompletedDealById;
using UseCases.CompleteDeal.Queries.GetCompletedDealsByClientId;
using UseCases.CompleteDeal.Queries.GetCompletedDealsByPropertyId;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCasesCompletedDealDto = UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления завершенными сделками
    /// Реализует CRUD операции для сущности CompletedDeal
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedDealsController : ControllerBase
    {
        private readonly ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity> _createCompleteDealHandler;
        private readonly ICommandHandler<DeleteCompletedDealCommand> _deleteCompletedDealHandler;
        private readonly IQueryHandler<GetCompletedDealByIdQuery, Result<UseCasesCompletedDealDto>> _getCompletedDealByIdHandler;
        private readonly IQueryHandler<GetAllCompletedDealsQuery, Result<IEnumerable<UseCasesCompletedDealDto>>> _getAllCompletedDealsHandler;
        private readonly IQueryHandler<GetCompletedDealsByClientIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>> _getCompletedDealsByClientIdHandler;
        private readonly IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>> _getCompletedDealsByPropertyIdHandler;
        private readonly IMapper _mapper;

        public CompletedDealsController(
            ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity> createCompleteDealHandler,
            ICommandHandler<DeleteCompletedDealCommand> deleteCompletedDealHandler,
            IQueryHandler<GetCompletedDealByIdQuery, Result<UseCasesCompletedDealDto>> getCompletedDealByIdHandler,
            IQueryHandler<GetAllCompletedDealsQuery, Result<IEnumerable<UseCasesCompletedDealDto>>> getAllCompletedDealsHandler,
            IQueryHandler<GetCompletedDealsByClientIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>> getCompletedDealsByClientIdHandler,
            IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<UseCasesCompletedDealDto>>> getCompletedDealsByPropertyIdHandler,
            IMapper mapper)
        {
            _createCompleteDealHandler = createCompleteDealHandler;
            _deleteCompletedDealHandler = deleteCompletedDealHandler;
            _getCompletedDealByIdHandler = getCompletedDealByIdHandler;
            _getAllCompletedDealsHandler = getAllCompletedDealsHandler;
            _getCompletedDealsByClientIdHandler = getCompletedDealsByClientIdHandler;
            _getCompletedDealsByPropertyIdHandler = getCompletedDealsByPropertyIdHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новую завершенную сделку
        /// </summary>
        /// <param name="request">Данные для создания завершенной сделки</param>
        /// <returns>Созданная завершенная сделка</returns>
        /// <response code="201">Завершенная сделка успешно создана</response>
        /// <response code="400">Ошибка валидации данных</response>

        [HttpPost]
        public async Task<Envelope> CreateCompletedDeal([FromBody] CreateCompletedDealRequest request)
        {
            var command = _mapper.Map<CreateCompleteDealCommand>(request);

            var result = await _createCompleteDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var response = _mapper.Map<CompletedDealDto>(result.Value);
            return new Envelope(HttpStatusCode.Created, response);
        }

        /// <summary>
        /// Получает завершенную сделку по ID
        /// </summary>
        /// <param name="id">Идентификатор завершенной сделки</param>
        /// <returns>Завершенная сделка</returns>
        /// <response code="200">Завершенная сделка найдена</response>
        /// <response code="404">Завершенная сделка не найдена</response>
        [HttpGet("{id}")]
        public async Task<Envelope> GetCompletedDeal(Guid id)
        {
            var query = new GetCompletedDealByIdQuery(id);
            var result = await _getCompletedDealByIdHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            var response = _mapper.Map<CompletedDealDto>(result.Value);
            return new Envelope(response);
        }

        /// <summary>
        /// Получает все завершенные сделки
        /// </summary>
        /// <returns>Список всех завершенных сделок</returns>
        /// <response code="200">Список завершенных сделок успешно получен</response>
        /// <response code="400">Ошибка получения списка</response>
        [HttpGet]
        public async Task<Envelope> GetAllCompletedDeals()
        {
            var result = await _getAllCompletedDealsHandler.HandleAsync(new GetAllCompletedDealsQuery());
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var items = _mapper.Map<IEnumerable<CompletedDealDto>>(result.Value);
            return new Envelope(items);
        }

        /// <summary>
        /// Получает завершенные сделки по ID клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список завершенных сделок клиента</returns>
        /// <response code="200">Список завершенных сделок клиента успешно получен</response>
        /// <response code="400">Ошибка получения списка</response>
        [HttpGet("by-client/{clientId}")]
        public async Task<Envelope> GetCompletedDealsByClient(Guid clientId)
        {
            var query = new GetCompletedDealsByClientIdQuery(clientId);
            var result = await _getCompletedDealsByClientIdHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var items = _mapper.Map<IEnumerable<CompletedDealDto>>(result.Value);
            return new Envelope(items);
        }

        /// <summary>
        /// Получает завершенные сделки по ID объекта недвижимости
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <returns>Список завершенных сделок по объекту недвижимости</returns>
        /// <response code="200">Список завершенных сделок успешно получен</response>
        /// <response code="400">Ошибка получения списка</response>
        [HttpGet("by-property/{propertyId}")]
        public async Task<Envelope> GetCompletedDealsByProperty(Guid propertyId)
        {
            var query = new GetCompletedDealsByPropertyIdQuery(propertyId);
            var result = await _getCompletedDealsByPropertyIdHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var items = _mapper.Map<IEnumerable<CompletedDealDto>>(result.Value);
            return new Envelope(items);
        }

        /// <summary>
        /// Удаляет завершенную сделку по ID
        /// </summary>
        /// <param name="id">Идентификатор завершенной сделки</param>
        /// <returns></returns>
        /// <response code="204">Завершенная сделка успешно удалена</response>
        /// <response code="404">Завершенная сделка не найдена</response>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteCompletedDeal(Guid id)
        {
            var command = new DeleteCompletedDealCommand(id);
            var result = await _deleteCompletedDealHandler.HandleAsync(command);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent, result: null);
        }
    }
}
