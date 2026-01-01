using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.DTOs.ClientDTO;
using Presenter.Extensions;
using Presenter.Utilities;
using UseCases.Client.Commands;
using UseCases.Client.Commands.CreateClient;
using UseCases.Client.Commands.DeleteClient;
using UseCases.Client.Commands.UpdateClient;
using UseCases.Client.Queries;
using UseCases.Client.Queries.GetAllClient;
using UseCases.Client.Queries.GetClientById;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;

namespace Presenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с клиентами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ICommandHandler<CreateClientCommand, Domain.Customers.Client.ClientEntity> _createClientCommandHandler;
        private readonly ICommandHandler<UpdateClientCommand, Domain.Customers.Client.ClientEntity> _updateClientCommandHandler;
        private readonly ICommandHandler<DeleteClientCommand, Domain.Customers.Client.ClientEntity> _deleteClientCommandHandler;
        private readonly IQueryHandler<GetClientByIdQuery, CSharpFunctionalExtensions.Result<Domain.Customers.Client.ClientEntity>> _getClientByIdQueryHandler;
        private readonly IQueryHandler<GetAllClientsQuery, CSharpFunctionalExtensions.Result<System.Collections.Generic.IEnumerable<Domain.Customers.Client.ClientEntity>>> _getAllClientsQueryHandler;

        /// <summary>
        /// Конструктор контроллера клиентов
        /// </summary>
        /// <param name="createClientCommandHandler">Обработчик команды создания клиента</param>
        /// <param name="updateClientCommandHandler">Обработчик команды обновления клиента</param>
        /// <param name="deleteClientCommandHandler">Обработчик команды удаления клиента</param>
        /// <param name="getClientByIdQueryHandler">Обработчик запроса получения клиента по ID</param>
        /// <param name="getAllClientsQueryHandler">Обработчик запроса получения всех клиентов</param>
        public ClientsController(
            ICommandHandler<CreateClientCommand, Domain.Customers.Client.ClientEntity> createClientCommandHandler,
            ICommandHandler<UpdateClientCommand, Domain.Customers.Client.ClientEntity> updateClientCommandHandler,
            ICommandHandler<DeleteClientCommand, Domain.Customers.Client.ClientEntity> deleteClientCommandHandler,
            IQueryHandler<GetClientByIdQuery, CSharpFunctionalExtensions.Result<Domain.Customers.Client.ClientEntity>> getClientByIdQueryHandler,
            IQueryHandler<GetAllClientsQuery, CSharpFunctionalExtensions.Result<System.Collections.Generic.IEnumerable<Domain.Customers.Client.ClientEntity>>> getAllClientsQueryHandler)
        {
            _createClientCommandHandler = createClientCommandHandler;
            _updateClientCommandHandler = updateClientCommandHandler;
            _deleteClientCommandHandler = deleteClientCommandHandler;
            _getClientByIdQueryHandler = getClientByIdQueryHandler;
            _getAllClientsQueryHandler = getAllClientsQueryHandler;
        }

        /// <summary>
        /// Создание нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <returns>Созданный клиент</returns>
        [HttpPost]
        public async Task<Envelope> CreateClient([FromBody] CreateClientRequest request)
        {
            var command = new CreateClientCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber);

            var result = await _createClientCommandHandler.HandleAsync(command);

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
            var query = new GetClientByIdQuery(id);
            var result = await _getClientByIdQueryHandler.HandleAsync(query);
            
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
            var query = new GetAllClientsQuery();
            var result = await _getAllClientsQueryHandler.HandleAsync(query);
            
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
        public async Task<Envelope> UpdateClient(Guid id, [FromBody] UpdateClientRequest clientRequest)
        {
            var command = new UpdateClientCommand(
                id,
                clientRequest.FirstName,
                clientRequest.LastName,
                clientRequest.Email,
                clientRequest.PhoneNumber);

            var result = await _updateClientCommandHandler.HandleAsync(command);

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
            var command = new DeleteClientCommand(id);
            var result = await _deleteClientCommandHandler.HandleAsync(command);
            
            if (result.IsFailure)
            {
                return new Envelope(HttpStatusCode.BadRequest, error: result.Error);
            }

            return new Envelope(result.Value.ToDTO());
        }
    }
}