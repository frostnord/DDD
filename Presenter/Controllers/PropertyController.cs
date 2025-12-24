using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Property.VO;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.PropertyDTO;
using Presenter.Extensions;
using Presenter.Utilities;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;
using UseCases.Property;

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
        private readonly IPropertyService _propertyService;

        /// <summary>
        /// Инициализирует новый экземпляр класса PropertyController.
        /// </summary>
        /// <param name="createPropertyHandler">Обработчик команды создания недвижимости</param>
        /// <param name="propertyService">Сервис для операций чтения/обновления/поиска недвижимости</param>
        public PropertyController(
            ICommandHandler<CreatePropertyCommand, Guid> createPropertyHandler,
            IPropertyService propertyService)
        {
            _createPropertyHandler = createPropertyHandler;
            _propertyService = propertyService;
        }

        /// <summary>
        /// Создает новый объект недвижимости.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные об объекте недвижимости</param>
        /// <returns>Созданный объект недвижимости с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<Envelope> CreateProperty([FromBody] CreatePropertyRequest request)
        {
            var command = new CreatePropertyCommand(
                request.Address.Street,
                request.Address.City,
                request.Address.HomeNumber,
                request.Address.ZipCode,
                request.Address.Country,
                request.PropertyDetails.Price,
                request.PropertyDetails.Description,
                request.PropertyDetails.NumberOfRooms,
                request.PropertyDetails.Floor,
                request.PropertyDetails.TotalFloors,
                request.PropertyDetails.Type,
                request.PropertyDetails.Heating,
                request.PropertyDetails.Condition,
                request.PropertyDetails.Area,
                request.PropertyDetails.HasParking,
                request.Ownership.OwnerClientId,
                request.Ownership.StartDate);

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
            var result = await _propertyService.GetPropertyByIdAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: result.Error);
            }

            return new Envelope(result.Value.ToDTO());
        }

        /// <summary>
        /// Получает все объекты недвижимости с опциональной фильтрацией.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации объектов недвижимости</param>
        /// <returns>Список объектов недвижимости, соответствующих критериям фильтрации, с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<Envelope> GetProperties([FromQuery] SearchPropertiesQuery query)
        {
            var result = await _propertyService.SearchPropertiesAsync(
                query.City, query.PropertyType, query.MinPrice, query.MaxPrice, query.MinArea, query.MaxArea,
                query.MinRooms, query.MaxRooms, query.MinFloor, query.MaxFloor, query.HeatingType,
                query.PropertyCondition, query.HasParking);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            var properties = result.Value.Select(p => p.ToDTO()).ToList();
            return new Envelope(properties);
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
            var getPropertyResult = await _propertyService.GetPropertyByIdAsync(id);
            if (getPropertyResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: getPropertyResult.Error);
            }

            var updateResult = await _propertyService.UpdatePropertyAsync(id, request.Street, request.City,
                request.HomeNumber, request.ZipCode, request.Country, request.Price, request.Description,
                request.NumberOfRooms, request.Floor, request.TotalFloors, request.PropertyType, request.HeatingType,
                request.PropertyCondition, request.Area, request.HasParking);
            if (updateResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: updateResult.Error);
            }

            // После обновления получаем обновленное свойство
            var getResult = await _propertyService.GetPropertyByIdAsync(id);
            if (getResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: getResult.Error);
            }

            return new Envelope(getResult.Value.ToDTO());
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

            return new Envelope(HttpStatusCode.NoContent, null);
        }
    }
}