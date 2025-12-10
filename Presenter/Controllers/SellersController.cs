using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Seller;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Interfaces;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с продавцами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SellersController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        /// <summary>
        /// Конструктор контроллера продавцов
        /// </summary>
        /// <param name="sellerService">Сервис для работы с продавцами</param>
        public SellersController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        /// <summary>
        /// Создание нового продавца
        /// </summary>
        /// <param name="request">Данные для создания продавца</param>
        /// <returns>Созданный продавец</returns>
        [HttpPost]
        public async Task<ActionResult<SellerDto>> CreateSeller([FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.CreateSellerAsync(request.ClientId);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetSeller),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        /// <summary>
        /// Получение продавца по ID
        /// </summary>
        /// <param name="id">Идентификатор продавца</param>
        /// <returns>Продавец</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SellerDto>> GetSeller(Guid id)
        {
            var result = await _sellerService.GetSellerByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }

        /// <summary>
        /// Получение списка всех продавцов
        /// </summary>
        /// <returns>Список продавцов</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellerDto>>> GetSellers()
        {
            var result = await _sellerService.GetAllSellersAsync();
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.Select(seller => seller.ToDTO()));
        }

        /// <summary>
        /// Обновление информации о продавце
        /// </summary>
        /// <param name="id">Идентификатор продавца</param>
        /// <param name="request">Данные для обновления продавца</param>
        /// <returns>Обновленный продавец</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<SellerDto>> UpdateSeller(Guid id, [FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.UpdateSellerAsync(id, request.ClientId);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            // Возвращаем обновленного продавца
            var updatedSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (updatedSellerResult.IsFailure)
            {
                return BadRequest(new { Error = updatedSellerResult.Error });
            }

            return Ok(updatedSellerResult.Value.ToDTO());
        }

        /// <summary>
        /// Удаление продавца
        /// </summary>
        /// <param name="id">Идентификатор продавца</param>
        /// <returns>Удаленный продавец</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<SellerDto>> DeleteSeller(Guid id)
        {
            var getSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (getSellerResult.IsFailure)
            {
                return BadRequest(new { Error = getSellerResult.Error });
            }

            var result = await _sellerService.DeleteSellerAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(getSellerResult.Value.ToDTO());
        }
    }
}


