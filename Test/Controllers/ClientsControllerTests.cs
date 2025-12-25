using System;
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
using Presenter.Utilities;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;
using Xunit;

namespace Test.Controllers
{
    public class ClientsControllerTests
    {
        private readonly Mock<IClientService> _mockClientService;
        private readonly ClientsController _controller;

        public ClientsControllerTests()
        {
            _mockClientService = new Mock<IClientService>();
            _controller = new ClientsController(_mockClientService.Object);
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

            // Создаем клиента через фабричный метод, используя данные из запроса
            var client = ClientEntity.Create(
                Name.Create(request.FirstName).Value,
                Name.Create(request.LastName).Value,
                ContactInfo.Create(
                    Email.Create(request.Email).Value,
                    PhoneNumber.Create(request.PhoneNumber).Value).Value
            ).Value;

            var result = Result.Success(client);
            _mockClientService.Setup(x => x.CreateClientAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateClient(request);

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
            _mockClientService.Setup(x => x.CreateClientAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateClient(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Validation error", envelope.Error.ToString());
        }

        [Fact]
        public async Task GetClient_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            // Создаем клиента через фабричный метод с фиксированными данными
            var client = ClientEntity.Create(
                Name.Create("Иван").Value,
                Name.Create("Иванов").Value,
                ContactInfo.Create(
                    Email.Create("ivan@example.com").Value,
                    PhoneNumber.Create("+79991234567").Value).Value
            ).Value;

            var result = Result.Success(client);
            _mockClientService.Setup(x => x.GetClientByIdAsync(clientId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetClient(clientId);

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
            _mockClientService.Setup(x => x.GetClientByIdAsync(clientId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetClient(clientId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Client not found", envelope.Error.ToString());
        }

        [Fact]
        public async Task UpdateClient_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var request = new CreateClientRequest
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Email = "ivan@example.com",
                PhoneNumber = "+79991234567"
            };

            // Создаем клиента через фабричный метод, используя данные из запроса
            var client = ClientEntity.Create(
                Name.Create(request.FirstName).Value,
                Name.Create(request.LastName).Value,
                ContactInfo.Create(
                    Email.Create(request.Email).Value,
                    PhoneNumber.Create(request.PhoneNumber).Value).Value
            ).Value;

            var result = Result.Success(client);
            _mockClientService.Setup(x => x.UpdateClientAsync(
                    clientId,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.UpdateClient(clientId, request);

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

            // Мокаем успешное удаление
            _mockClientService.Setup(x => x.DeleteClientAsync(clientId))
                .ReturnsAsync(Result.Success());

            // Создаем клиента через фабричный метод с фиксированными данными
            var client = ClientEntity.Create(
                Name.Create("Иван").Value,
                Name.Create("Иванов").Value,
                ContactInfo.Create(
                    Email.Create("ivan@example.com").Value,
                    PhoneNumber.Create("+79991234567").Value).Value
            ).Value;

            _mockClientService.Setup(x => x.GetClientByIdAsync(clientId))
                .ReturnsAsync(Result.Success(client));

            // Act
            var actionResult = await _controller.DeleteClient(clientId);

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