using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Interfaces;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        /// <summary>
        /// Создание клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientRequest request)
        {
            var result = await _clientService.CreateClientAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber);
            
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetClient),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }
        /// <summary>
        /// Получение клиента по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(Guid id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }
        /// <summary>
        /// Получение всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var result = await _clientService.GetAllClientsAsync();
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.Select(client => client.ToDTO()));
        }
        /// <summary>
        /// Обновить клиента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromBody] CreateClientRequest clientDto)
        {
            var result = await _clientService.UpdateClientAsync(
                id,
                clientDto.FirstName,
                clientDto.LastName,
                clientDto.Email,
                clientDto.PhoneNumber);
                
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }
        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ClientDto>> DeleteClient(Guid id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            // Для возврата удаленного клиента, нужно сначала получить его
            var getClientResult = await _clientService.GetClientByIdAsync(id);
            if (getClientResult.IsFailure)
            {
                return NotFound(new { Error = getClientResult.Error });
            }

            return Ok(getClientResult.Value.ToDTO());
        }
    }
}