using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.PropertyDTO;
using Presenter.Mappings;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using PropertyDto = UseCases.Property.Queries.GetPropertyById.PropertyDto;


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
        private readonly IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>> _getPropertyByIdHandler;
        private readonly IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>> _searchPropertiesHandler;

        /// <summary>
        /// Инициализирует новый экземпляр класса PropertyController.
        /// </summary>
        /// <param name="createPropertyHandler">Обработчик команды создания недвижимости</param>
        /// <param name="getPropertyByIdHandler">Обработчик запроса на получение недвижимости по ID</param>
        /// <param name="searchPropertiesHandler">Обработчик запроса на поиск недвижимости</param>
        public PropertyController(
            ICommandHandler<CreatePropertyCommand, Guid> createPropertyHandler,
            IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>> getPropertyByIdHandler,
            IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>> searchPropertiesHandler)
        {
            _createPropertyHandler = createPropertyHandler;
            _getPropertyByIdHandler = getPropertyByIdHandler;
            _searchPropertiesHandler = searchPropertiesHandler;
        }

        /// <summary>
        /// Создает новый объект недвижимости.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные об объекте недвижимости</param>
        /// <returns>Созданный объект недвижимости с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<Envelope> CreateProperty([FromBody] CreatePropertyRequest request)
        {
            var command = CreatePropertyMapping.ToCommand(request);
            var result = await _createPropertyHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value);
        }

        /// <summary>
        /// Получает объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="query">Уникальный идентификатор объекта недвижимости</param>
        /// <returns>Запрошенный объект недвижимости с HTTP 200 если найден, иначе HTTP 404 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetProperty(GetPropertyByIdQuery query)
        {
            var result = await _getPropertyByIdHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }
            
            var propertyDto = result.Value;

            return new Envelope(propertyDto);
        }

        /// <summary>
        /// Получает все объекты недвижимости с опциональной фильтрацией.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации объектов недвижимости</param>
        /// <returns>Список объектов недвижимости, соответствующих критериям фильтрации, с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<Envelope> GetProperties([FromQuery] SearchPropertiesQuery query)
        {
            var result = await _searchPropertiesHandler.HandleAsync(query);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var searchResult = result.Value;
            
            return new Envelope(new
            {
                Items = searchResult.Items,
                TotalCount = searchResult.TotalCount,
                PageSize = searchResult.PageSize,
                TotalPages = searchResult.TotalPages,
                CurrentPage = query.Page
            });
        }

        /// <summary>
        /// Обновляет существующий объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные объекта недвижимости</param>
        /// <returns>Обновленный объект недвижимости с HTTP 200 при успешном выполнении, иначе HTTP 400 или 404 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request)
        {
            // Для обновления сначала получаем свойство, обновляем его и сохраняем
            var query = new GetPropertyByIdQuery(id);
            var getPropertyResult = await _getPropertyByIdHandler.HandleAsync(query);
            if (getPropertyResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: getPropertyResult.Error);
            }

            var updateResult = await _propertyService.UpdatePropertyAsync(id, request);
            if (updateResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: updateResult.Error);
            }

            // После обновления получаем обновленное свойство
            var updatedPropertyResult = await _getPropertyByIdHandler.HandleAsync(query);
            if (updatedPropertyResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: updatedPropertyResult.Error);
            }

            var updatedProperty = updatedPropertyResult.Value;
            var updatedPropertyDto = updatedProperty.ToDTO();

            return new Envelope(updatedPropertyDto);
        }

        /// <summary>
        /// Удаляет объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для удаления</param>
        /// <returns>HTTP 204 при успешном удалении, иначе HTTP 404 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteProperty(Guid id)
        {
            var result = await _propertyService.DeletePropertyAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            // После удаления возвращаем NoContent
            return new Envelope(HttpStatusCode.NoContent, null);
        }
    }
}