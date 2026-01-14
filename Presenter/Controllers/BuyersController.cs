using System;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.Buyer;
using Presenter.DTOs.BuyerDTO;
using UseCases.Buyer;
using UseCases.Buyer.Commands.CreateBuyer;
using UseCases.Buyer.Commands.DeleteBuyer;
using UseCases.Buyer.Commands.UpdateBuyer;
using UseCases.Buyer.Queries.GetBuyerById;
using UseCases.Buyer.Queries.SearchBuyersQuery;
using AutoMapper;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления сущностями покупателей в системе управления недвижимостью.
    /// Предоставляет конечные точки для создания, получения, обновления и удаления покупателей.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly ICommandHandler<CreateBuyerCommand, Guid> _createBuyerHandler;
        private readonly ICommandHandler<UpdateBuyerCommand> _updateBuyerHandler;
        private readonly ICommandHandler<DeleteBuyerCommand> _deleteBuyerHandler;
        private readonly IQueryHandler<GetBuyerByIdQuery, Result<BuyerDto>> _getBuyerByIdHandler;
        private readonly IQueryHandler<SearchBuyersQuery, Result<SearchBuyersQueryResponse>> _searchBuyersHandler;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр класса BuyersController.
        /// </summary>
        public BuyersController(
            ICommandHandler<CreateBuyerCommand, Guid> createBuyerHandler,
            ICommandHandler<UpdateBuyerCommand> updateBuyerHandler,
            ICommandHandler<DeleteBuyerCommand> deleteBuyerHandler,
            IQueryHandler<GetBuyerByIdQuery, Result<BuyerDto>> getBuyerByIdHandler,
            IQueryHandler<SearchBuyersQuery, Result<SearchBuyersQueryResponse>> searchBuyersHandler,
            IMapper mapper)
        {
            _createBuyerHandler = createBuyerHandler;
            _updateBuyerHandler = updateBuyerHandler;
            _deleteBuyerHandler = deleteBuyerHandler;
            _getBuyerByIdHandler = getBuyerByIdHandler;
            _searchBuyersHandler = searchBuyersHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает нового покупателя.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные о покупателе</param>
        /// <returns>ID созданного покупателя с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<Envelope> CreateBuyer([FromBody] CreateBuyerRequest request)
        {
            var command = _mapper.Map<CreateBuyerCommand>(request);

            var result = await _createBuyerHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value);
        }

        /// <summary>
        /// Получает покупателя по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор покупателя</param>
        /// <returns>Запрошенный покупатель с HTTP 200 если найден, иначе HTTP 404 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetBuyer(Guid id)
        {
            var query = new GetBuyerByIdQuery(id);
            var result = await _getBuyerByIdHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(result.Value);
        }

        /// <summary>
        /// Получает всех покупателей с опциональной фильтрацией.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации покупателей</param>
        /// <returns>Список покупателей, соответствующих критериям фильтрации, с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<Envelope> GetBuyers([FromQuery] SearchBuyersQuery query)
        {
            var result = await _searchBuyersHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var searchResult = result.Value;
            var response = new PagedBuyersResponse(
                searchResult.Items,
                searchResult.TotalCount,
                searchResult.PageSize,
                searchResult.TotalPages,
                query.Page);

            return new Envelope(response);
        }

        /// <summary>
        /// Обновляет существующего покупателя по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор покупателя для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные покупателя</param>
        /// <returns>HTTP 204 при успешном выполнении, иначе HTTP 400 или 404 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateBuyer(Guid id, [FromBody] UpdateBuyerRequest request)
        {
            var command = _mapper.Map<UpdateBuyerCommand>(request, opt =>
                opt.Items["Id"] = id);

            var result = await _updateBuyerHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Удаляет покупателя по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор покупателя для удаления</param>
        /// <returns>HTTP 204 при успешном удалении, иначе HTTP 404 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteBuyer(Guid id)
        {
            var command = new DeleteBuyerCommand(id);
            var result = await _deleteBuyerHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }
    }
}