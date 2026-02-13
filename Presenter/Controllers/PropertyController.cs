using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.PropertyDTO;
using Presenter.DTOs.PropertyDTO.Request.CreatePoperty;
using Presenter.DTOs.PropertyDTO.Request.UpdateProperty;
using Presenter.DTOs.PropertyDTO.Response;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Commands.DeleteProperty;
using UseCases.Property.Commands.UpdateProperty;
using UseCases.Property.Queries;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.Reservation.Queries;
using UseCases.UseCases.DTO.Booking;
using UseCases.UseCases.DTO.Property;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для управления объектами недвижимости в системе управления недвижимостью.
    /// Предоставляет конечные точки для создания, получения, обновления и удаления объектов недвижимости.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly ICommandHandler<CreatePropertyCommand, Guid> _createPropertyHandler;
        private readonly ICommandHandler<UpdatePropertyCommand> _updatePropertyHandler;
        private readonly ICommandHandler<DeletePropertyCommand> _deletePropertyHandler;
        private readonly IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>> _getPropertyByIdHandler;
        private readonly IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>> _searchPropertiesHandler;
        private readonly IQueryHandler<GetPropertyReservationQuery, Result<ReservationDto>> _getPropertyReservationHandler;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр класса PropertyController.
        /// </summary>
        public PropertyController(
            ICommandHandler<CreatePropertyCommand, Guid> createPropertyHandler,
            ICommandHandler<UpdatePropertyCommand> updatePropertyHandler,
            ICommandHandler<DeletePropertyCommand> deletePropertyHandler,
            IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>> getPropertyByIdHandler,
            IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>> searchPropertiesHandler,
            IQueryHandler<GetPropertyReservationQuery, Result<ReservationDto>> getPropertyReservationHandler,
            IMapper mapper)
        {
            _createPropertyHandler = createPropertyHandler;
            _updatePropertyHandler = updatePropertyHandler;
            _deletePropertyHandler = deletePropertyHandler;
            _getPropertyByIdHandler = getPropertyByIdHandler;
            _searchPropertiesHandler = searchPropertiesHandler;
            _getPropertyReservationHandler = getPropertyReservationHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новый объект недвижимости.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные об объекте недвижимости</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданный объект недвижимости с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<Envelope> CreateProperty([FromBody] CreatePropertyRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreatePropertyCommand>(request);
            var result = await _createPropertyHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value);
        }

        /// <summary>
        /// Получает объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Запрошенный объект недвижимости с HTTP 200 если найден, иначе HTTP 404 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetProperty(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetPropertyByIdQuery(id);
            var result = await _getPropertyByIdHandler.HandleAsync(query, cancellationToken);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            var response = _mapper.Map<PropertyResponse>(result.Value);
            return new Envelope(response);
        }

        [HttpGet("{id}/reservation")]
        public async Task<Envelope> GetPropertyReservation(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetPropertyReservationQuery(id);
            var result = await _getPropertyReservationHandler.HandleAsync(query, cancellationToken);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(result.Value);
        }

        /// <summary>
        /// Получает все объекты недвижимости с опциональной фильтрацией.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации объектов недвижимости</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список объектов недвижимости, соответствующих критериям фильтрации, с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<Envelope> GetProperties([FromQuery] SearchPropertiesQuery query, CancellationToken cancellationToken)
        {
            var result = await _searchPropertiesHandler.HandleAsync(query, cancellationToken);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var searchResult = result.Value;
            var items = _mapper.Map<IEnumerable<PropertyResponse>>(searchResult.Items);
            var response = new PagedPropertiesResponse(
                items,
                searchResult.TotalCount,
                searchResult.PageSize,
                searchResult.TotalPages,
                query.Page);
            
            return new Envelope(response);
        }

        /// <summary>
        /// Обновляет существующий объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные объекта недвижимости</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>HTTP 204 при успешном выполнении, иначе HTTP 400 или 404 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdatePropertyCommand>(request, opt => 
                opt.Items["Id"] = id);
            
            var result = await _updatePropertyHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Удаляет объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для удаления</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>HTTP 204 при успешном удалении, иначе HTTP 404 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteProperty(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeletePropertyCommand(id);
            var result = await _deletePropertyHandler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }
    }
}