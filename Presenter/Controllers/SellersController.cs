using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.SellerDTO;
using Presenter.Extensions;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления сущностями продавцов в системе управления недвижимостью.
    /// Предоставляет конечные точки для создания, получения, обновления и удаления продавцов.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SellersController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        /// <summary>
        /// Инициализирует новый экземпляр класса SellersController.
        /// </summary>
        /// <param name="sellerService">Сервис для обработки бизнес-логики, связанной с продавцами</param>
        public SellersController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        /// <summary>
        /// Создает нового продавца.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные о продавце</param>
        /// <returns>Созданный продавец с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
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
        /// Получает продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца</param>
        /// <returns>Запрошенный продавец с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
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
        /// Получает всех продавцов.
        /// </summary>
        /// <returns>Список всех продавцов с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
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
        /// Обновляет существующего продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные продавца</param>
        /// <returns>Обновленный продавец с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<SellerDto>> UpdateSeller(Guid id, [FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.UpdateSellerAsync(id, request.ClientId);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            var updatedSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (updatedSellerResult.IsFailure)
            {
                return BadRequest(new { Error = updatedSellerResult.Error });
            }

            return Ok(updatedSellerResult.Value.ToDTO());
        }

        /// <summary>
        /// Удаляет продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца для удаления</param>
        /// <returns>Удаленный продавец с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
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