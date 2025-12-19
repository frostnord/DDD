using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.DealDTO;
using Presenter.Extensions;
using UseCases.CompleteDeal;
using UseCases.Deal;
using UseCases.Deal.Commands;
using UseCases.Interfaces;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления сделками в системе недвижимости
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly ICommandHandler<CreateDealCommand, DealEntity> _createDealHandler;
        private readonly ICommandHandler<ConfirmDealCommand> _confirmDealHandler;
        private readonly ICommandHandler<CompleteDealCommand> _completeDealHandler;
        private readonly ICommandHandler<CancelDealCommand> _cancelDealHandler;
        private readonly IDealService _dealService;

        /// <summary>
        /// Конструктор контроллера сделок
        /// </summary>
        /// <param name="createDealHandler">Обработчик команды создания сделки</param>
        /// <param name="confirmDealHandler">Обработчик команды подтверждения сделки</param>
        /// <param name="completeDealHandler">Обработчик команды завершения сделки</param>
        /// <param name="cancelDealHandler">Обработчик команды отмены сделки</param>
        /// <param name="dealRepository">Репозиторий для работы с сущностями сделок</param>
        public DealsController(
            ICommandHandler<CreateDealCommand, DealEntity> createDealHandler,
            ICommandHandler<ConfirmDealCommand> confirmDealHandler,
            ICommandHandler<CompleteDealCommand> completeDealHandler,
            ICommandHandler<CancelDealCommand> cancelDealHandler,
            IDealRepository dealRepository, IDealService dealService)
        {
            _createDealHandler = createDealHandler;
            _confirmDealHandler = confirmDealHandler;
            _completeDealHandler = completeDealHandler;
            _cancelDealHandler = cancelDealHandler;
            _dealService = dealService;
        }

        /// <summary>
        /// Создает новую сделку
        /// </summary>
        /// <param name="request">Данные для создания сделки</param>
        /// <returns>Созданная сделка</returns>
        /// <response code="201">Возвращает созданную сделку</response>
        /// <response code="400">Если данные для создания сделки некорректны</response>
        [HttpPost]
        public async Task<ActionResult<DealDto>> CreateDeal([FromBody] CreateDealRequest request)
        {
            var command = new CreateDealCommand
            {
                ClientId = request.ClientId,
                PropertyId = request.PropertyId,
                BookingId = request.BookingId,
                Details = request.Details
            };

            var result = await _createDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetDeal),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        /// <summary>
        /// Возвращает сделку по её идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <returns>Информация о сделке</returns>
        /// <response code="200">Возвращает информацию о сделке</response>
        /// <response code="400">Если идентификатор сделки некорректен</response>
        /// <response code="404">Если сделка не найдена</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<DealDto>> GetDeal(Guid id)
        {
            var dealResult = await _dealService.GetByIdAsync(id);
            if (dealResult.IsFailure)
            {
                return NotFound(new { Error = "Deal not found" });
            }

            // Проверка на null для дополнительной безопасности
            if (dealResult.Value == null)
            {
                return NotFound(new { Error = "Deal not found" });
            }

            var dealDto = dealResult.Value.ToDTO();
            return Ok(dealDto);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetDeals([FromQuery] SearchDealsQuery query)
        {
            IEnumerable<DealEntity> deals;

            // Если заданы фильтры по клиенту или недвижимости, используем соответствующие методы сервиса
            if (query.ClientId.HasValue)
            {
                var dealsResult = await _dealService.GetByClientIdAsync(query.ClientId.Value);
                if (dealsResult.IsFailure)
                {
                    return BadRequest(new { Error = dealsResult.Error });
                }
                
                deals = dealsResult.Value;
            }
            else if (query.PropertyId.HasValue)
            {
                var dealsResult = await _dealService.GetByPropertyIdAsync(query.PropertyId.Value);
                if (dealsResult.IsFailure)
                {
                    return BadRequest(new { Error = dealsResult.Error });
                }
                
                deals = dealsResult.Value;
            }
            else
            {
                return BadRequest(new { Error = "Нужен id клиента или недвижимости" });

            }
            
            var dealDtos = deals.Select(d => d.ToDTO()).ToList();
            
            return Ok(new
            {
                Items = dealDtos,
            });
        }

        /// <summary>
        /// Подтверждает сделку
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <returns>Результат подтверждения сделки</returns>
        /// <response code="200">Если сделка успешно подтверждена</response>
        /// <response code="400">Если идентификатор сделки некорректен или произошла ошибка при подтверждении</response>
        [HttpPut("{id}/confirm")]
        public async Task<ActionResult> ConfirmDeal(Guid id)
        {
            var command = new ConfirmDealCommand { DealId = id };
            var result = await _confirmDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(new { Message = "Deal confirmed successfully" });
        }

        /// <summary>
        /// Завершает сделку
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <returns>Результат завершения сделки</returns>
        /// <response code="200">Если сделка успешно завершена</response>
        /// <response code="400">Если идентификатор сделки некорректен или произошла ошибка при завершении</response>
        [HttpPut("{id}/complete")]
        public async Task<ActionResult> CompleteDeal(Guid id)
        {
            var command = new CompleteDealCommand { DealId = id };
            var result = await _completeDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(new { Message = "Deal completed successfully" });
        }

        /// <summary>
        /// Отменяет сделку
        /// </summary>
        /// <param name="id">Идентификатор сделки</param>
        /// <returns>Результат отмены сделки</returns>
        /// <response code="200">Если сделка успешно отменена</response>
        /// <response code="400">Если идентификатор сделки некорректен или произошла ошибка при отмене</response>
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelDeal(Guid id)
        {
            var command = new CancelDealCommand { DealId = id };
            var result = await _cancelDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(new { Message = "Deal cancelled successfully" });
        }
    }
}