using System.Net;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.SellerDTO;
using Presenter.Extensions;
using Presenter.Utilities;
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
        public async Task<Envelope> CreateSeller([FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.CreateSellerAsync(request.ClientId);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value.ToDTO());
        }

        /// <summary>
        /// Получает продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца</param>
        /// <returns>Запрошенный продавец с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetSeller(Guid id)
        {
            var result = await _sellerService.GetSellerByIdAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.ToDTO());
        }

        /// <summary>
        /// Получает всех продавцов.
        /// </summary>
        /// <returns>Список всех продавцов с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<Envelope> GetSellers()
        {
            var result = await _sellerService.GetAllSellersAsync();
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.Select(seller => seller.ToDTO()));
        }

        /// <summary>
        /// Обновляет существующего продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные продавца</param>
        /// <returns>Обновленный продавец с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateSeller(Guid id, [FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.UpdateSellerAsync(id, request.ClientId);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var updatedSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (updatedSellerResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: updatedSellerResult.Error);
            }

            return new Envelope(updatedSellerResult.Value.ToDTO());
        }

        /// <summary>
        /// Удаляет продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца для удаления</param>
        /// <returns>Удаленный продавец с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteSeller(Guid id)
        {
            var getSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (getSellerResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: getSellerResult.Error);
            }

            var result = await _sellerService.DeleteSellerAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(getSellerResult.Value.ToDTO());
        }
    }
}