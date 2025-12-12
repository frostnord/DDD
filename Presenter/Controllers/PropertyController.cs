using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.PropertyDTO;
using Presenter.Extensions;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

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