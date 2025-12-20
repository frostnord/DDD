using System.Net;
using System.Collections.Generic;
using System.Linq;
using Domain.Deal;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.CompletedDealDTO;
using Presenter.Extensions;
using Presenter.Utilities;
using UseCases.CompleteDeal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

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
        private readonly ICompletedDealService _completedDealService;

        public CompletedDealsController(
            ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity> createCompleteDealHandler,
            ICompletedDealService completedDealService)
        {
            _createCompleteDealHandler = createCompleteDealHandler;
            _completedDealService = completedDealService;
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
            var command = new CreateCompleteDealCommand
            {
                BuyerClientId = request.BuyerClientId,
                SellerClientId = request.SellerClientId,
                PropertyId = request.PropertyId,
                DealDate = request.DealDate,
                DealAmount = request.DealAmount,
                DealType = request.DealType
            };

            var result = await _createCompleteDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value.ToDto());
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
            var result = await _completedDealService.GetByIdAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(result.Value.ToDto());
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
            var result = await _completedDealService.GetAllAsync();
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.Select(d => d.ToDto()));
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
            var result = await _completedDealService.GetByClientIdAsync(clientId);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.Select(d => d.ToDto()));
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
            var result = await _completedDealService.GetByPropertyIdAsync(propertyId);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.Select(d => d.ToDto()));
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
            var result = await _completedDealService.DeleteAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent, null);
        }
    }
}
