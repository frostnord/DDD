```csharp
using System.Net;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs.PropertyDTO;
using Presenter.DTOs.PropertyDTO.CreatePoperty;
using Presenter.DTOs.PropertyDTO.UpdateProperty;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Commands.DeleteProperty;
using UseCases.Property.Commands.UpdateProperty;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
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
            IMapper mapper)
        {
            _createPropertyHandler = createPropertyHandler;
            _updatePropertyHandler = updatePropertyHandler;
            _deletePropertyHandler = deletePropertyHandler;
            _getPropertyByIdHandler = getPropertyByIdHandler;
            _searchPropertiesHandler = searchPropertiesHandler;
            _mapper = mapper;
        }

        /// <summary>
        /// Создает новый объект недвижимости.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные об объекте недвижимости</param>
        /// <returns>Созданный объект недвижимости с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<Envelope> CreateProperty([FromBody] CreatePropertyRequest request)
        {
            var command = _mapper.Map<CreatePropertyCommand>(request);
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
        /// <param name="id">Уникальный идентификатор объекта недвижимости</param>
        /// <returns>Запрошенный объект недвижимости с HTTP 200 если найден, иначе HTTP 404 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetProperty(Guid id)
        {
            var query = new GetPropertyByIdQuery(id);
            var result = await _getPropertyByIdHandler.HandleAsync(query);
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
            var response = new PagedPropertiesResponse
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
        /// Обновляет существующий объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные объекта недвижимости</param>
        /// <returns>HTTP 204 при успешном выполнении, иначе HTTP 400 или 404 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request)
        {
            var command = _mapper.Map<UpdatePropertyCommand>(request, opt => 
                opt.Items["Id"] = id);
            
            var result = await _updatePropertyHandler.HandleAsync(command);

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
        /// <returns>HTTP 204 при успешном удалении, иначе HTTP 404 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteProperty(Guid id)
        {
            var command = new DeletePropertyCommand(id);
            var result = await _deletePropertyHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(HttpStatusCode.NoContent);
        }
    }
}
```

---

## Основные требования к контроллерам

1.  **CQRS**:
    *   Все операции, изменяющие состояние (Create, Update, Delete), должны использовать **команды** (`ICommandHandler`).
    *   Все операции чтения данных (Get, Search) должны использовать **запросы** (`IQueryHandler`).
    *   Контроллер не должен содержать бизнес-логики. Его задача — принять HTTP-запрос, передать его соответствующему обработчику и вернуть HTTP-ответ.

2.  **Внедрение зависимостей (DI)**:
    *   Все зависимости (обработчики команд/запросов, `IMapper`) должны внедряться через конструктор.

3.  **DTO (Data Transfer Objects)**:
    *   Для каждого типа запроса (создание, обновление) должен использоваться свой DTO (`Create...Request`, `Update...Request`).
    *   Данные из запроса должны мапиться в команды/запросы с помощью `AutoMapper`.

4.  **Обработка ответов**:
    *   Все публичные методы контроллера должны возвращать `Task<Envelope>`.
    *   `Envelope` используется для стандартизации ответов API.
    *   При успешном выполнении операции возвращается `new Envelope(result)` или `new Envelope(HttpStatusCode, result)`.
    *   При ошибке возвращается `new Envelope(HttpStatusCode, error: result.Error)`.

5.  **REST-принципы**:
    *   Используйте стандартные HTTP-методы: `POST` для создания, `GET` для получения, `PUT` для обновления, `DELETE` для удаления.
    *   Используйте стандартные HTTP-статус-коды: `201 Created`, `200 OK`, `204 NoContent`, `400 BadRequest`, `404 NotFound`.
    *   URL должны быть интуитивно понятными и следовать формату `api/[controller]`. Для получения/обновления/удаления конкретного ресурса используйте `/{id}`.
