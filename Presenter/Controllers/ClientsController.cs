using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Commands;
using UseCases.Interfaces;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с клиентами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        /// <summary>
        /// Конструктор контроллера клиентов
        /// </summary>
        /// <param name="clientService">Сервис для работы с клиентами</param>
        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Создание нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <returns>Созданный клиент</returns>
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
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Клиент</returns>
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
        /// Получение списка всех клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
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
        /// Обновление информации о клиенте
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="clientRequest">Данные для обновления клиента</param>
        /// <returns>Обновленный клиент</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromBody] CreateClientRequest clientRequest)
        {
            var result = await _clientService.UpdateClientAsync(
                id,
                clientRequest.FirstName,
                clientRequest.LastName,
                clientRequest.Email,
                clientRequest.PhoneNumber);
                
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Удаленный клиент</returns>
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