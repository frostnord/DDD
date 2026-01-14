using System;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.SellerDTO;
using Presenter.Utilities;
using UseCases.DTO.Seller;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Seller;
using UseCases.Seller.Commands;
using UseCases.Seller.Queries;

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
        private readonly ICommandHandler<CreateSellerCommand, Guid> _createSellerHandler;
        private readonly ICommandHandler<UpdateSellerCommand> _updateSellerHandler;
        private readonly ICommandHandler<DeleteSellerCommand> _deleteSellerHandler;
        private readonly IQueryHandler<GetSellerByIdQuery, Result<SellerDto>> _getSellerByIdHandler;
        private readonly IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>> _searchSellersHandler;

        /// <summary>
        /// Инициализирует новый экземпляр класса SellersController.
        /// </summary>
        public SellersController(
            ICommandHandler<CreateSellerCommand, Guid> createSellerHandler,
            ICommandHandler<UpdateSellerCommand> updateSellerHandler,
            ICommandHandler<DeleteSellerCommand> deleteSellerHandler,
            IQueryHandler<GetSellerByIdQuery, Result<SellerDto>> getSellerByIdHandler,
            IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>> searchSellersHandler)
        {
            _createSellerHandler = createSellerHandler;
            _updateSellerHandler = updateSellerHandler;
            _deleteSellerHandler = deleteSellerHandler;
            _getSellerByIdHandler = getSellerByIdHandler;
            _searchSellersHandler = searchSellersHandler;
        }

        /// <summary>
        /// Создает нового продавца.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные о продавце</param>
        /// <returns>ID созданного продавца с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<Envelope> CreateSeller([FromBody] CreateSellerRequest request)
        {
            var command = new CreateSellerCommand(request.ClientId);
            var result = await _createSellerHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value);
        }

        /// <summary>
        /// Получает продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца</param>
        /// <returns>Запрошенный продавец с HTTP 200 если найден, иначе HTTP 404 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetSeller(Guid id)
        {
            var query = new GetSellerByIdQuery(id);
            var result = await _getSellerByIdHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }
            
            return new Envelope(result.Value);
        }

        /// <summary>
        /// Получает всех продавцов с опциональной фильтрацией.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации продавцов</param>
        /// <returns>Список продавцов, соответствующих критериям фильтрации, с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<Envelope> GetSellers([FromQuery] SearchSellersQuery query)
        {
            var result = await _searchSellersHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var searchResult = result.Value;
            var response = new PagedSellersResponse
            {
                Items = searchResult.Items,
                TotalCount = searchResult.TotalCount,
                PageSize = searchResult.PageSize,
                TotalPages = searchResult.TotalPages,
                CurrentPage = query.Page
            };

            return new Envelope(response);
        }

        /// <summary>
        /// Обновляет существующего продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные продавца</param>
        /// <returns>HTTP 204 при успешном выполнении, иначе HTTP 400 или 404 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateSeller(Guid id, [FromBody] UpdateSellerRequest request)
        {
            var command = new UpdateSellerCommand(id, request.ClientId);
            var result = await _updateSellerHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Удаляет продавца по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор продавца для удаления</param>
        /// <returns>HTTP 204 при успешном удалении, иначе HTTP 404 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteSeller(Guid id)
        {
            var command = new DeleteSellerCommand(id);
            var result = await _deleteSellerHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }
    }
}