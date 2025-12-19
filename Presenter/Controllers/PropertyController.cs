using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.PropertyDTO;
using Presenter.Extensions;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;

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
        private readonly IPropertyService _propertyService;

        /// <summary>
        /// Инициализирует новый экземпляр класса PropertyController.
        /// </summary>
        /// <param name="propertyService">Сервис для обработки бизнес-логики, связанной с недвижимостью</param>
        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// Создает новый объект недвижимости.
        /// </summary>
        /// <param name="request">Запрос, содержащий данные об объекте недвижимости</param>
        /// <returns>Созданный объект недвижимости с HTTP 201 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpPost]
        public async Task<ActionResult<PropertyDto>> CreateProperty([FromBody] CreatePropertyRequest request)
        {
            var result = await _propertyService.CreatePropertyAsync(
                request.Street,
                request.City,
                request.HomeNumber,
                request.ZipCode,
                request.Country,
                request.Price,
                request.Description,
                request.NumberOfRooms,
                request.Floor,
                request.TotalFloors,
                request.PropertyType,
                request.HeatingType,
                request.PropertyCondition,
                request.Area,
                request.HasParking,
                request.OwnerClientId,
                request.StartDate
            );

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetProperty),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        /// <summary>
        /// Получает объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости</param>
        /// <returns>Запрошенный объект недвижимости с HTTP 20 если найден, иначе HTTP 404 с деталями ошибки</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetProperty(Guid id)
        {
            var result = await _propertyService.GetPropertyByIdAsync(id);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }

        /// <summary>
        /// Получает все объекты недвижимости с опциональной фильтрацией.
        /// </summary>
        /// <param name="query">Параметры запроса для фильтрации объектов недвижимости</param>
        /// <returns>Список объектов недвижимости, соответствующих критериям фильтрации, с HTTP 200 при успешном выполнении, иначе HTTP 400 с деталями ошибки</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties([FromQuery] SearchPropertiesQuery query)
        {
            var result = await _propertyService.SearchPropertiesAsync(
                query.City, query.PropertyType, query.MinPrice, query.MaxPrice, query.MinArea, query.MaxArea,
                query.MinRooms, query.MaxRooms, query.MinFloor, query.MaxFloor, query.HeatingType,
                query.PropertyCondition, query.HasParking);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            var properties = result.Value.Select(p => p.ToDTO()).ToList();
            return Ok(properties);
        }

        /// <summary>
        /// Обновляет существующий объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для обновления</param>
        /// <param name="request">Запрос, содержащий обновленные данные объекта недвижимости</param>
        /// <returns>Обновленный объект недвижимости с HTTP 20 при успешном выполнении, иначе HTTP 400 или 404 с деталями ошибки</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertyDto>> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request)
        {
            // Для обновления сначала получаем свойство, обновляем его и сохраняем
            var getPropertyResult = await _propertyService.GetPropertyByIdAsync(id);
            if (getPropertyResult.IsFailure)
            {
                return NotFound(new { Error = getPropertyResult.Error });
            }

            var updateResult = await _propertyService.UpdatePropertyAsync(id, request.Street, request.City,
                request.HomeNumber, request.ZipCode, request.Country, request.Price, request.Description,
                request.NumberOfRooms, request.Floor, request.TotalFloors, request.PropertyType, request.HeatingType,
                request.PropertyCondition, request.Area, request.HasParking);
            if (updateResult.IsFailure)
            {
                return BadRequest(new { Error = updateResult.Error });
            }

            // После обновления получаем обновленное свойство
            var getResult = await _propertyService.GetPropertyByIdAsync(id);
            if (getResult.IsFailure)
            {
                return NotFound(new { Error = getResult.Error });
            }

            return Ok(getResult.Value.ToDTO());
        }

        /// <summary>
        /// Удаляет объект недвижимости по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта недвижимости для удаления</param>
        /// <returns>HTTP 204 при успешном удалении, иначе HTTP 404 с деталями ошибки</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProperty(Guid id)
        {
            var result = await _propertyService.DeletePropertyAsync(id);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return NoContent();
        }
    }
}