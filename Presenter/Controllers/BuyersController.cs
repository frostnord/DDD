using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.BuyerDTO;
using Presenter.Extensions;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с покупателями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerService _buyerService;

        /// <summary>
        /// Конструктор контроллера покупателей
        /// </summary>
        /// <param name="buyerService">Сервис для работы с покупателями</param>
        public BuyersController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        /// <summary>
        /// Создание нового покупателя
        /// </summary>
        /// <param name="request">Данные для создания покупателя</param>
        /// <returns>Созданный покупатель</returns>
        [HttpPost]
        public async Task<ActionResult<BuyerDto>> CreateBuyer([FromBody] CreateBuyerRequest request)
        {
            var result = await _buyerService.CreateBuyerAsync(request.ClientId, request.PreferredNumberOfRooms,
                request.PreferredFloor, request.PreferredTotalFloors, request.PreferredType,
                request.PreferredHeatingType, request.PreferredCondition, request.PreferParking);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetBuyer),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        /// <summary>
        /// Получение покупателя по ID
        /// </summary>
        /// <param name="id">Идентификатор покупателя</param>
        /// <returns>Покупатель</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BuyerDto>> GetBuyer(Guid id)
        {
            var result = await _buyerService.GetBuyerByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }

        /// <summary>
        /// Получение списка всех покупателей
        /// </summary>
        /// <returns>Список покупателей</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuyerDto>>> GetBuyers()
        {
            var result = await _buyerService.GetAllBuyersAsync();
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.Select(buyer => buyer.ToDTO()));
        }

        /// <summary>
        /// Обновление информации о покупателе
        /// </summary>
        /// <param name="id">Идентификатор покупателя</param>
        /// <param name="request">Данные для обновления покупателя</param>
        /// <returns>Обновленный покупатель</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BuyerDto>> UpdateBuyer(Guid id, [FromBody] CreateBuyerRequest request)
        {
            var result = await _buyerService.UpdateBuyerAsync(id, request.ClientId, request.PreferredNumberOfRooms,
                request.PreferredFloor, request.PreferredTotalFloors, request.PreferredType,
                request.PreferredHeatingType, request.PreferredCondition, request.PreferParking);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            // Возвращаем обновленного покупателя
            var updatedBuyerResult = await _buyerService.GetBuyerByIdAsync(id);
            if (updatedBuyerResult.IsFailure)
            {
                return BadRequest(new { Error = updatedBuyerResult.Error });
            }

            return Ok(updatedBuyerResult.Value.ToDTO());
        }

        /// <summary>
        /// Удаление покупателя
        /// </summary>
        /// <param name="id">Идентификатор покупателя</param>
        /// <returns>Удаленный покупатель</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BuyerDto>> DeleteBuyer(Guid id)
        {
            var getBuyerResult = await _buyerService.GetBuyerByIdAsync(id);
            if (getBuyerResult.IsFailure)
            {
                return BadRequest(new { Error = getBuyerResult.Error });
            }

            var result = await _buyerService.DeleteBuyerAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(getBuyerResult.Value.ToDTO());
        }
    }
}