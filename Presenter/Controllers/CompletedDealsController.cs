using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Microsoft.AspNetCore.Mvc;
using UseCases.Deal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedDealsController : ControllerBase
    {
        private readonly ICommandHandler<CreateCompleteDealCommand, CompletedDeal>
            _createCompleteDealHandler;

        private readonly ICompletedDealRepository _completedDealRepository;

        public CompletedDealsController(
            ICommandHandler<CreateCompleteDealCommand, CompletedDeal> createCompleteDealHandler,
            ICompletedDealRepository completedDealRepository)
        {
            _createCompleteDealHandler = createCompleteDealHandler;
            _completedDealRepository = completedDealRepository;
        }

        [HttpPost]
        public async Task<ActionResult<CompletedDeal>> CreateCompletedDeal(
            [FromBody] CreateCompleteDealCommand command)
        {
            var result = await _createCompleteDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetCompletedDeal),
                new { id = result.Value.Id.Value },
                result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompletedDeal>> GetCompletedDeal(Guid id)
        {
            var dealId = CompletedDealId.Create(id);
            if (dealId.IsFailure)
            {
                return BadRequest(new { Error = "Invalid deal ID" });
            }

            var result = await _completedDealRepository.GetByIdAsync(dealId.Value);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompletedDeal>>> GetAllCompletedDeals()
        {
            // В реальной реализации нужно будет добавить метод в репозиторий для получения всех сделок
            // Пока что возвращаем пустой список
            return Ok(new List<CompletedDeal>());
        }

        [HttpGet("by-client/{clientId}")]
        public async Task<ActionResult<IEnumerable<CompletedDeal>>> GetCompletedDealsByClient(
            Guid clientId)
        {
            var clientIdVO = ClientId.Create(clientId);
            if (clientIdVO.IsFailure)
            {
                return BadRequest(new { Error = "Invalid client ID" });
            }

            var result = await _completedDealRepository.GetByClientIdAsync(clientIdVO.Value);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpGet("by-property/{propertyId}")]
        public async Task<ActionResult<IEnumerable<CompletedDeal>>> GetCompletedDealsByProperty(
            Guid propertyId)
        {
            var propertyIdVO = PropertyId.Create(propertyId);
            if (propertyIdVO.IsFailure)
            {
                return BadRequest(new { Error = "Invalid property ID" });
            }

            var result = await _completedDealRepository.GetByPropertyIdAsync(propertyIdVO.Value);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompletedDeal(Guid id, [FromBody] CompletedDeal updatedDeal)
        {
            var dealId = CompletedDealId.Create(id);
            if (dealId.IsFailure)
            {
                return BadRequest(new { Error = "Invalid deal ID" });
            }

            if (updatedDeal.Id.Value != id)
            {
                return BadRequest(new { Error = "Deal ID in the URL does not match the ID in the body" });
            }

            var result = await _completedDealRepository.UpdateAsync(updatedDeal);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCompletedDeal(Guid id)
        {
            var dealId = CompletedDealId.Create(id);
            if (dealId.IsFailure)
            {
                return BadRequest(new { Error = "Invalid deal ID" });
            }

            var result = await _completedDealRepository.DeleteAsync(dealId.Value);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return NoContent();
        }
    }
}