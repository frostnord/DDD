using System.Net;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.ClientDTO;
using Presenter.Extensions;
using Presenter.Utilities;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;

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
        public async Task<Envelope> CreateClient([FromBody] CreateClientRequest request)
        {
            var result = await _clientService.CreateClientAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(HttpStatusCode.Created, result.Value.ToDTO());
        }

        /// <summary>
        /// Получение клиента по ID
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Клиент</returns>
        [HttpGet("{id}")]
        public async Task<Envelope> GetClient(Guid id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.ToDTO());
        }

        /// <summary>
        /// Получение списка всех клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        [HttpGet]
        public async Task<Envelope> GetClients()
        {
            var result = await _clientService.GetAllClientsAsync();
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.Select(client => client.ToDTO()));
        }

        /// <summary>
        /// Обновление информации о клиенте
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="clientRequest">Данные для обновления клиента</param>
        /// <returns>Обновленный клиент</returns>
        [HttpPut("{id}")]
        public async Task<Envelope> UpdateClient(Guid id, [FromBody] CreateClientRequest clientRequest)
        {
            var result = await _clientService.UpdateClientAsync(
                id,
                clientRequest.FirstName,
                clientRequest.LastName,
                clientRequest.Email,
                clientRequest.PhoneNumber);

            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.ToDTO());
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Удаленный клиент</returns>
        [HttpDelete("{id}")]
        public async Task<Envelope> DeleteClient(Guid id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            // Для возврата удаленного клиента, нужно сначала получить его
            var getClientResult = await _clientService.GetClientByIdAsync(id);
            if (getClientResult.IsFailure)
            {
                return new Envelope(HttpStatusCode.NotFound, error: getClientResult.Error);
            }

            return new Envelope(getClientResult.Value.ToDTO());
        }
    }
}