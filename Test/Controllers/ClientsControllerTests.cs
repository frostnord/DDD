using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
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
using Xunit;

namespace Test.Controllers
{
    public class ClientsControllerTests
    {
        private readonly Mock<ICommandHandler<CreateClientCommand, ClientEntity>> _mockCreateClientCommandHandler;
        private readonly Mock<ICommandHandler<UpdateClientCommand, ClientEntity>> _mockUpdateClientCommandHandler;
        private readonly Mock<ICommandHandler<DeleteClientCommand, ClientEntity>> _mockDeleteClientCommandHandler;
        private readonly Mock<IQueryHandler<GetClientByIdQuery, Result<ClientEntity>>> _mockGetClientByIdQueryHandler;
        private readonly Mock<IQueryHandler<GetAllClientsQuery, Result<IEnumerable<ClientEntity>>>> _mockGetAllClientsQueryHandler;
        private readonly ClientsController _controller;

        public ClientsControllerTests()
        {
            _mockCreateClientCommandHandler = new Mock<ICommandHandler<CreateClientCommand, ClientEntity>>();
            _mockUpdateClientCommandHandler = new Mock<ICommandHandler<UpdateClientCommand, ClientEntity>>();
            _mockDeleteClientCommandHandler = new Mock<ICommandHandler<DeleteClientCommand, ClientEntity>>();
            _mockGetClientByIdQueryHandler = new Mock<IQueryHandler<GetClientByIdQuery, Result<ClientEntity>>>();
            _mockGetAllClientsQueryHandler = new Mock<IQueryHandler<GetAllClientsQuery, Result<IEnumerable<ClientEntity>>>>();

            _controller = new ClientsController(
                _mockCreateClientCommandHandler.Object,
                _mockUpdateClientCommandHandler.Object,
                _mockDeleteClientCommandHandler.Object,
                _mockGetClientByIdQueryHandler.Object,
                _mockGetAllClientsQueryHandler.Object);
        }


        [Fact]
        public async Task CreateClient_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateClientRequest
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Email = "ivan@example.com",
                PhoneNumber = "+79991234567"
            };

            var client = ClientEntity.Create(
                Name.Create(request.FirstName).Value,
                Name.Create(request.LastName).Value,
                ContactInfo.Create(
                    Email.Create(request.Email).Value,
                    PhoneNumber.Create(request.PhoneNumber).Value).Value
            ).Value;

            var result = Result.Success(client);
            _mockCreateClientCommandHandler.Setup(x => x.HandleAsync(
                    It.Is<CreateClientCommand>(cmd => cmd.FirstName == request.FirstName &&
                                                     cmd.LastName == request.LastName &&
                                                     cmd.Email == request.Email &&
                                                     cmd.PhoneNumber == request.PhoneNumber),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateClient(request, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(201, envelope.Status);
            var clientDto = Assert.IsType<ClientDto>(envelope.Result);
            Assert.Equal("Иван", clientDto.FirstName);
            Assert.Equal("Иванов", clientDto.LastName);
            Assert.Equal("ivan@example.com", clientDto.Email);
        }

        [Fact]
        public async Task CreateClient_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateClientRequest
            {
                FirstName = "",
                LastName = "",
                Email = "",
                PhoneNumber = ""
            };

            var errorResult = Result.Failure<ClientEntity>("Validation error");
            _mockCreateClientCommandHandler.Setup(x => x.HandleAsync(
                    It.Is<CreateClientCommand>(cmd => cmd.FirstName == request.FirstName &&
                                                     cmd.LastName == request.LastName &&
                                                     cmd.Email == request.Email &&
                                                     cmd.PhoneNumber == request.PhoneNumber),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateClient(request, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Validation error", envelope.Error ?? string.Empty);
        }

        [Fact]
        public async Task GetClient_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            var client = ClientEntity.Create(
                Name.Create("Иван").Value,
                Name.Create("Иванов").Value,
                ContactInfo.Create(
                    Email.Create("ivan@example.com").Value,
                    PhoneNumber.Create("+79991234567").Value).Value
            ).Value;

            var result = Result.Success(client);
            _mockGetClientByIdQueryHandler.Setup(x => x.HandleAsync(
                    It.Is<GetClientByIdQuery>(q => q.ClientId.Equals(clientId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetClient(clientId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var clientDto = Assert.IsType<ClientDto>(envelope.Result);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal("Иван", clientDto.FirstName);
            Assert.Equal("Иванов", clientDto.LastName);
            Assert.Equal("ivan@example.com", clientDto.Email);
        }

        [Fact]
        public async Task GetClient_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var errorResult = Result.Failure<ClientEntity>("Client not found");
            _mockGetClientByIdQueryHandler.Setup(x => x.HandleAsync(
                    It.Is<GetClientByIdQuery>(q => q.ClientId.Equals(clientId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetClient(clientId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Client not found", envelope.Error ?? string.Empty);
        }

        [Fact]
        public async Task UpdateClient_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var request = new UpdateClientRequest
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Email = "ivan@example.com",
                PhoneNumber = "+79991234567"
            };

            var client = ClientEntity.Create(
                Name.Create(request.FirstName).Value,
                Name.Create(request.LastName).Value,
                ContactInfo.Create(
                    Email.Create(request.Email).Value,
                    PhoneNumber.Create(request.PhoneNumber).Value).Value
            ).Value;

            var result = Result.Success(client);
            _mockUpdateClientCommandHandler.Setup(x => x.HandleAsync(
                    It.Is<UpdateClientCommand>(cmd => cmd.ClientId == clientId &&
                                                     cmd.FirstName == request.FirstName &&
                                                     cmd.LastName == request.LastName &&
                                                     cmd.Email == request.Email &&
                                                     cmd.PhoneNumber == request.PhoneNumber),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.UpdateClient(clientId, request, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var clientDto = Assert.IsType<ClientDto>(envelope.Result);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal("Иван", clientDto.FirstName);
            Assert.Equal("Иванов", clientDto.LastName);
            Assert.Equal("ivan@example.com", clientDto.Email);
        }

        [Fact]
        public async Task DeleteClient_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            var client = ClientEntity.Create(
                Name.Create("Иван").Value,
                Name.Create("Иванов").Value,
                ContactInfo.Create(
                    Email.Create("ivan@example.com").Value,
                    PhoneNumber.Create("+79991234567").Value).Value
            ).Value;

            _mockDeleteClientCommandHandler
                .Setup(x => x.HandleAsync(
                    It.Is<DeleteClientCommand>(cmd => cmd.ClientId == clientId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(client));

            // Act
            var actionResult = await _controller.DeleteClient(clientId, CancellationToken.None);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var clientDto = Assert.IsType<ClientDto>(envelope.Result);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal("Иван", clientDto.FirstName);
            Assert.Equal("Иванов", clientDto.LastName);
            Assert.Equal("ivan@example.com", clientDto.Email);
        }
    }
}