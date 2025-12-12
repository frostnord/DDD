using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Interfaces;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellersController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellersController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpPost]
        public async Task<ActionResult<SellerDto>> CreateSeller([FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.CreateSellerAsync(request.ClientId);

            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetSeller),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SellerDto>> GetSeller(Guid id)
        {
            var result = await _sellerService.GetSellerByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellerDto>>> GetSellers()
        {
            var result = await _sellerService.GetAllSellersAsync();
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.Select(seller => seller.ToDTO()));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SellerDto>> UpdateSeller(Guid id, [FromBody] CreateSellerRequest request)
        {
            var result = await _sellerService.UpdateSellerAsync(id, request.ClientId);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            var updatedSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (updatedSellerResult.IsFailure)
            {
                return BadRequest(new { Error = updatedSellerResult.Error });
            }

            return Ok(updatedSellerResult.Value.ToDTO());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<SellerDto>> DeleteSeller(Guid id)
        {
            var getSellerResult = await _sellerService.GetSellerByIdAsync(id);
            if (getSellerResult.IsFailure)
            {
                return BadRequest(new { Error = getSellerResult.Error });
            }

            var result = await _sellerService.DeleteSellerAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(getSellerResult.Value.ToDTO());
        }
        
    }
}
             



