using Domain.Deal;
using Microsoft.AspNetCore.Mvc;
using UseCases.Clients.Commands;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly ICommandHandler<CreateDealCommand, DealEntity> _createDealHandler;

        public DealsController(ICommandHandler<CreateDealCommand, DealEntity> createDealHandler)
        {
            _createDealHandler = createDealHandler;
        }

        [HttpPost]
        public async Task<ActionResult<DealEntity>> CreateDeal([FromBody] CreateDealCommand command)
        {
            var result = await _createDealHandler.HandleAsync(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetDeal),
                new { id = result.Value.Id.Value },
                result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DealEntity>> GetDeal(Guid id)
        {
            var dealId = DealId.Create(id);
            if (dealId.IsFailure)
            {
                return BadRequest(new { Error = "Invalid deal ID" });
            }

            // Заглушка - в реальной реализации нужно получить объект из репозитория
            return Ok(); // Возвращаем Ok, так как конкретный объект создать сложно без полного конструктора
        }
    }
}