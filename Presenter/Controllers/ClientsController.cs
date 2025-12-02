using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Commands;
using UseCases.Handlers;
using UseCases.Interfaces.Repositories;

namespace Presenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ICommandHandler<CreateClientCommand, Client> _createClientHandler;

        public ClientsController(
            ICommandHandler<CreateClientCommand, Client> createClientHandler)
        {
            _createClientHandler = createClientHandler;
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto request)
        {
            var command = new CreateClientCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
                
            };
            
 
            var result = ClientStorage.Add(command);
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetClient),
                new { id = result.Value.Id.Value },
                result.Value.ToDTO());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(Guid id)
        {
            var clientIdResult = ClientId.Create(id);
            if (clientIdResult.IsFailure)
            {
                return BadRequest(new { Error = "Invalid client ID" });
            }

            var clientId = clientIdResult.Value;
            var result = ClientStorage.Get(clientId);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(result.Value.ToDTO());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var result = ClientStorage.GetAllDtos();
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromBody] CreateClientDto clientDto)
        {
            var clientIdResult = ClientId.Create(id);
            if (clientIdResult.IsFailure)
            {
                return BadRequest(new { Error = "Invalid client ID" });
            }

            var clientId = clientIdResult.Value;
            var existingClientResult = ClientStorage.Get(clientId);
            if (existingClientResult.IsFailure)
            {
                return NotFound(new { Error = existingClientResult.Error });
            }

            var existingClient = existingClientResult.Value;

            var firstNameResult = Name.Create(clientDto.FirstName);
            if (firstNameResult.IsFailure)
                return BadRequest(new { Error = firstNameResult.Error });
    
            var lastNameResult = Name.Create(clientDto.LastName);
            if (lastNameResult.IsFailure)
                return BadRequest(new { Error = lastNameResult.Error });
    
            var emailResult = Email.Create(clientDto.Email);
            if (emailResult.IsFailure)
                return BadRequest(new { Error = emailResult.Error });
    
            var phoneResult = PhoneNumber.Create(clientDto.PhoneNumber);
            if (phoneResult.IsFailure)
                return BadRequest(new { Error = phoneResult.Error });
    
            var contactInfoResult = ContactInfo.Create(emailResult.Value, phoneResult.Value);
            if (contactInfoResult.IsFailure)
                return BadRequest(new { Error = contactInfoResult.Error });
    
            var updateResult = existingClient.UpdateClientData(firstNameResult.Value, lastNameResult.Value, contactInfoResult.Value);
            if (updateResult.IsFailure)
                return BadRequest(new { Error = updateResult.Error });
    
            var result = ClientStorage.Update(clientId, new CreateClientCommand
            {
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                Email = clientDto.Email,
                PhoneNumber = clientDto.PhoneNumber
            });
            if (result.IsFailure)
            {
                return BadRequest(new { Error = result.Error });
            }
    
            return Ok(existingClient.ToDTO());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ClientDto>> DeleteClient(Guid id)
        {
            var clientIdResult = ClientId.Create(id);
            if (clientIdResult.IsFailure)
            {
                return BadRequest(new { Error = "Invalid client ID" });
            }

            var clientId = clientIdResult.Value;
            var getClientResult = ClientStorage.Get(clientId);
            if (getClientResult.IsFailure)
            {
                return NotFound(new { Error = getClientResult.Error });
            }

            var clientToDelete = getClientResult.Value;
            var result = ClientStorage.Remove(clientId);
            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }

            return Ok(clientToDelete.ToDTO());
        }

        [HttpGet("storage/{id}")]
       public async Task<ActionResult<ClientDto>> GetClientFromStorage(Guid id)
       {
           var clientIdResult = ClientId.Create(id);
           if (clientIdResult.IsFailure)
           {
               return BadRequest(new { Error = "Invalid client ID" });
           }

           var clientId = clientIdResult.Value;
           var result = ClientStorage.Get(clientId);
           if (result.IsFailure)
           {
               return NotFound(new { Error = result.Error });
           }

           return Ok(result.Value.ToDTO());
       }

       [HttpGet("storage")]
       public async Task<ActionResult<IEnumerable<ClientDto>>> GetClientsFromStorage()
       {
           var result = ClientStorage.GetAllDtos();
           if (result.IsFailure)
           {
               return BadRequest(new { Error = result.Error });
           }

           return Ok(result.Value);
       }

       [HttpPut("storage/{id}")]
       public async Task<ActionResult<ClientDto>> UpdateClientInStorage(Guid id, [FromBody] CreateClientDto clientDto)
       {
           var clientIdResult = ClientId.Create(id);
           if (clientIdResult.IsFailure)
           {
               return BadRequest(new { Error = "Invalid client ID" });
           }

           var clientId = clientIdResult.Value;
           var existingClientResult = ClientStorage.Get(clientId);
           if (existingClientResult.IsFailure)
           {
               return NotFound(new { Error = existingClientResult.Error });
           }

           var existingClient = existingClientResult.Value;

           var updateResult = ClientStorage.UpdateDto(clientId, new CreateClientCommand
           {
               FirstName = clientDto.FirstName,
               LastName = clientDto.LastName,
               Email = clientDto.Email,
               PhoneNumber = clientDto.PhoneNumber
           });
           if (updateResult.IsFailure)
           {
               return BadRequest(new { Error = updateResult.Error });
           }

           return Ok(updateResult.Value);
       }

       [HttpDelete("storage/{id}")]
       public async Task<ActionResult<ClientDto>> DeleteClientFromStorage(Guid id)
       {
           var clientIdResult = ClientId.Create(id);
           if (clientIdResult.IsFailure)
           {
               return BadRequest(new { Error = "Invalid client ID" });
           }

           var clientId = clientIdResult.Value;
           var getClientResult = ClientStorage.Get(clientId);
           if (getClientResult.IsFailure)
           {
               return NotFound(new { Error = getClientResult.Error });
           }

           var clientToDelete = getClientResult.Value;
           var result = ClientStorage.Remove(clientId);
           if (result.IsFailure)
           {
               return NotFound(new { Error = result.Error });
           }

           return Ok(clientToDelete.ToDTO());
       }
    }
}