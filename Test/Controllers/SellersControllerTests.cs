using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs;
using Presenter.DTOs.SellerDTO;
using UseCases.Interfaces;
using Presenter.Utilities;
using UseCases.Interfaces.Services;
using Xunit;

namespace Test.Controllers
{
    public class SellersControllerTests
    {
        private readonly Mock<ISellerService> _mockSellerService;
        private readonly SellersController _controller;

        public SellersControllerTests()
        {
            _mockSellerService = new Mock<ISellerService>();
            _controller = new SellersController(_mockSellerService.Object);
        }

        [Fact]
        public async Task CreateSeller_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var request = new CreateSellerRequest
            {
                ClientId = clientId
            };

            // Создаем продавца через фабричный метод, используя ClientId из запроса
            var sellerId = SellerId.Create(Guid.NewGuid()).Value;
            var clientIdObj = ClientId.Create(clientId).Value;
            var seller = SellerEntity.Create(clientIdObj).Value;

            var result = Result.Success(seller);
            _mockSellerService.Setup(x => x.CreateSellerAsync(request.ClientId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateSeller(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(201, envelope.Status);
            var sellerDto = Assert.IsType<SellerDto>(envelope.Result);
            Assert.Equal(clientId, sellerDto.ClientId);
        }

        [Fact]
        public async Task CreateSeller_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var clientId = Guid.Empty; // Невалидный ClientId
            var request = new CreateSellerRequest
            {
                ClientId = clientId
            };

            var errorResult = Result.Failure<SellerEntity>("Validation error");
            _mockSellerService.Setup(x => x.CreateSellerAsync(request.ClientId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateSeller(request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Validation error", envelope.Error.ToString());
        }

        [Fact]
        public async Task GetSeller_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var sellerId = Guid.NewGuid();

            // Создаем продавца через фабричный метод с фиксированными данными
            var clientId = Guid.NewGuid();
            var clientIdObj = ClientId.Create(clientId).Value;
            var seller = SellerEntity.Create(clientIdObj).Value;

            var result = Result.Success(seller);
            _mockSellerService.Setup(x => x.GetSellerByIdAsync(sellerId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetSeller(sellerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var sellerDto = Assert.IsType<SellerDto>(envelope.Result);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal(clientId, sellerDto.ClientId);
        }

        [Fact]
        public async Task GetSeller_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var errorResult = Result.Failure<SellerEntity>("Seller not found");
            _mockSellerService.Setup(x => x.GetSellerByIdAsync(sellerId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetSeller(sellerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            Assert.Equal(400, envelope.Status);
            Assert.Contains("Seller not found", envelope.Error.ToString());
        }

        [Fact]
        public async Task UpdateSeller_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateSellerRequest
            {
                ClientId = clientId
            };

            // Создаем продавца через фабричный метод, используя ClientId из запроса
            var clientIdObj = ClientId.Create(clientId).Value;
            var seller = SellerEntity.Create(clientIdObj).Value;

            var result = Result.Success(seller);
            _mockSellerService.Setup(x => x.UpdateSellerAsync(sellerId, request.ClientId))
                .ReturnsAsync(result);

            // Мокаем GetSellerByIdAsync, чтобы он возвращал обновленного продавца
            _mockSellerService.Setup(x => x.GetSellerByIdAsync(sellerId))
                .ReturnsAsync(Result.Success(seller));

            // Act
            var actionResult = await _controller.UpdateSeller(sellerId, request);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var sellerDto = Assert.IsType<SellerDto>(envelope.Result);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal(clientId, sellerDto.ClientId);
        }

        [Fact]
        public async Task DeleteSeller_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var sellerId = Guid.NewGuid();

            // Мокаем успешное удаление
            _mockSellerService.Setup(x => x.DeleteSellerAsync(sellerId))
                .ReturnsAsync(Result.Success());

            // Создаем продавца через фабричный метод с фиксированными данными
            var clientId = Guid.NewGuid();
            var clientIdObj = ClientId.Create(clientId).Value;
            var seller = SellerEntity.Create(clientIdObj).Value;

            _mockSellerService.Setup(x => x.GetSellerByIdAsync(sellerId))
                .ReturnsAsync(Result.Success(seller));

            // Act
            var actionResult = await _controller.DeleteSeller(sellerId);

            // Assert
            var envelope = Assert.IsType<Envelope>(actionResult);
            var sellerDto = Assert.IsType<SellerDto>(envelope.Result);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal(clientId, sellerDto.ClientId);
        }
    }
}