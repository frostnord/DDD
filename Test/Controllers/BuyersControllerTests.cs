using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs;
using UseCases.Interfaces;
using Xunit;

namespace Test.Controllers
{
    public class BuyersControllerTests
    {
        private readonly Mock<IBuyerService> _mockBuyerService;
        private readonly BuyersController _controller;

        public BuyersControllerTests()
        {
            _mockBuyerService = new Mock<IBuyerService>();
            _controller = new BuyersController(_mockBuyerService.Object);
        }

        [Fact]
        public async Task CreateBuyer_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var request = new CreateBuyerRequest
            {
                ClientId = clientId
            };

            // Создаем покупателя через фабричный метод, используя ClientId из запроса
            var buyerId = Domain.Domain.Customers.Buyer.VO.BuyerId.Create(Guid.NewGuid()).Value;
            var clientIdObj = Domain.Domain.Customers.Client.VO.ClientId.Create(clientId).Value;
            
            // Создаем критерии поиска по умолчанию
            var numberOfRooms = Domain.Domain.ValueObjects.NumberOfRooms.Create(2).Value;
            var floor = Domain.Domain.ValueObjects.Floor.Create(3).Value;
            var totalFloors = Domain.Domain.ValueObjects.TotalFloors.Create(9).Value;
            var propertyType = Domain.Domain.ValueObjects.SmartPropertyType.FromName("Apartment");
            var heatingType = Domain.Domain.Property.VO.HeatingType.Create("Central").Value;
            var condition = Domain.Domain.Property.VO.PropertyCondition.Create("Хорошее").Value;
            var searchCriteria = Domain.Domain.Customers.Client.VO.ClientSearchCriteria.Create(
                numberOfRooms, floor, totalFloors, propertyType, true, heatingType, condition).Value;
            
            var buyer = Domain.Domain.Customers.Buyer.Buyer.Create(clientIdObj, searchCriteria).Value;
            
            var result = Result.Success(buyer);
            _mockBuyerService.Setup(x => x.CreateBuyerAsync(request.ClientId, request.PreferredNumberOfRooms, request.PreferredFloor, request.PreferredTotalFloors, request.PreferredType, request.PreferredHeatingType, request.PreferredCondition, request.PreferParking))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateBuyer(request);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetBuyer", createdAtResult.ActionName);
        }

        [Fact]
        public async Task CreateBuyer_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var clientId = Guid.Empty; // Невалидный ClientId
            var request = new CreateBuyerRequest
            {
                ClientId = clientId
            };

            var numberOfRooms = Domain.Domain.ValueObjects.NumberOfRooms.Create(2).Value;
            var floor = Domain.Domain.ValueObjects.Floor.Create(3).Value;
            var totalFloors = Domain.Domain.ValueObjects.TotalFloors.Create(9).Value;
            var propertyType = Domain.Domain.ValueObjects.SmartPropertyType.FromName("Apartment");
            var heatingType = Domain.Domain.Property.VO.HeatingType.Create("Central").Value;
            var condition = Domain.Domain.Property.VO.PropertyCondition.Create("Хорошее").Value;
            var searchCriteria = Domain.Domain.Customers.Client.VO.ClientSearchCriteria.Create(
                numberOfRooms, floor, totalFloors, propertyType, true, heatingType, condition).Value;
            
            var errorResult = Result.Failure<Domain.Domain.Customers.Buyer.Buyer>("Validation error");
            _mockBuyerService.Setup(x => x.CreateBuyerAsync(request.ClientId, request.PreferredNumberOfRooms, request.PreferredFloor, request.PreferredTotalFloors, request.PreferredType, request.PreferredHeatingType, request.PreferredCondition, request.PreferParking))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.CreateBuyer(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Contains("Validation error", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetBuyer_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            
            // Создаем покупателя через фабричный метод с фиксированными данными
            var clientId = Guid.NewGuid();
            var clientIdObj = Domain.Domain.Customers.Client.VO.ClientId.Create(clientId).Value;
            
            // Создаем критерии поиска по умолчанию
            var numberOfRooms = Domain.Domain.ValueObjects.NumberOfRooms.Create(2).Value;
            var floor = Domain.Domain.ValueObjects.Floor.Create(3).Value;
            var totalFloors = Domain.Domain.ValueObjects.TotalFloors.Create(9).Value;
            var propertyType = Domain.Domain.ValueObjects.SmartPropertyType.FromName("Apartment");
            var heatingType = Domain.Domain.Property.VO.HeatingType.Create("Central").Value;
            var condition = Domain.Domain.Property.VO.PropertyCondition.Create("Хорошее").Value;
            var searchCriteria = Domain.Domain.Customers.Client.VO.ClientSearchCriteria.Create(
                numberOfRooms, floor, totalFloors, propertyType, true, heatingType, condition).Value;
            
            var buyer = Domain.Domain.Customers.Buyer.Buyer.Create(clientIdObj, searchCriteria).Value;
            
            var result = Result.Success(buyer);
            _mockBuyerService.Setup(x => x.GetBuyerByIdAsync(buyerId))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetBuyer(buyerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var buyerDto = Assert.IsType<BuyerDto>(okResult.Value);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal(clientId, buyerDto.ClientId);
        }

        [Fact]
        public async Task GetBuyer_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var errorResult = Result.Failure<Domain.Domain.Customers.Buyer.Buyer>("Buyer not found");
            _mockBuyerService.Setup(x => x.GetBuyerByIdAsync(buyerId))
                .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.GetBuyer(buyerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Contains("Buyer not found", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateBuyer_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateBuyerRequest
            {
                ClientId = clientId,
                PreferredNumberOfRooms = 2,
                PreferredFloor = 3,
                PreferredTotalFloors = 9,
                PreferredType = "Apartment",
                PreferredHeatingType = "Central",
                PreferredCondition = "Хорошее",
                PreferParking = true
            };

            // Создаем критерии поиска из данных запроса
            var numberOfRooms = Domain.Domain.ValueObjects.NumberOfRooms.Create(request.PreferredNumberOfRooms).Value;
            var floor = Domain.Domain.ValueObjects.Floor.Create(request.PreferredFloor).Value;
            var totalFloors = Domain.Domain.ValueObjects.TotalFloors.Create(request.PreferredTotalFloors).Value;
            var propertyType = Domain.Domain.ValueObjects.SmartPropertyType.FromName(request.PreferredType);
            var heatingType = Domain.Domain.Property.VO.HeatingType.Create(request.PreferredHeatingType).Value;
            var condition = Domain.Domain.Property.VO.PropertyCondition.Create(request.PreferredCondition).Value;
            var searchCriteria = Domain.Domain.Customers.Client.VO.ClientSearchCriteria.Create(
                numberOfRooms, floor, totalFloors, propertyType, request.PreferParking, heatingType, condition).Value;
            
            // Создаем покупателя через фабричный метод, используя ClientId и критерии из запроса
            var clientIdObj = Domain.Domain.Customers.Client.VO.ClientId.Create(clientId).Value;
            var buyer = Domain.Domain.Customers.Buyer.Buyer.Create(clientIdObj, searchCriteria).Value;
            
            var result = Result.Success(buyer);
            _mockBuyerService.Setup(x => x.UpdateBuyerAsync(buyerId, request.ClientId, request.PreferredNumberOfRooms, request.PreferredFloor, request.PreferredTotalFloors, request.PreferredType, request.PreferredHeatingType, request.PreferredCondition, request.PreferParking))
                .ReturnsAsync(result);
            
            // Мокаем вызов GetBuyerByIdAsync, который используется в контроллере после обновления
            _mockBuyerService.Setup(x => x.GetBuyerByIdAsync(buyerId))
                .ReturnsAsync(Result.Success(buyer));

            // Act
            var actionResult = await _controller.UpdateBuyer(buyerId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var buyerDto = Assert.IsType<BuyerDto>(okResult.Value);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal(clientId, buyerDto.ClientId);
        }

        [Fact]
        public async Task DeleteBuyer_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            
            // Мокаем успешное удаление
            _mockBuyerService.Setup(x => x.DeleteBuyerAsync(buyerId))
                .ReturnsAsync(Result.Success());
            
            // Создаем покупателя через фабричный метод с фиксированными данными
            var clientId = Guid.NewGuid();
            var clientIdObj = Domain.Domain.Customers.Client.VO.ClientId.Create(clientId).Value;
            
            // Создаем критерии поиска по умолчанию
            var numberOfRooms = Domain.Domain.ValueObjects.NumberOfRooms.Create(2).Value;
            var floor = Domain.Domain.ValueObjects.Floor.Create(3).Value;
            var totalFloors = Domain.Domain.ValueObjects.TotalFloors.Create(9).Value;
            var propertyType = Domain.Domain.ValueObjects.SmartPropertyType.FromName("Apartment");
            var heatingType = Domain.Domain.Property.VO.HeatingType.Create("Central").Value;
            var condition = Domain.Domain.Property.VO.PropertyCondition.Create("Хорошее").Value;
            var searchCriteria = Domain.Domain.Customers.Client.VO.ClientSearchCriteria.Create(
                numberOfRooms, floor, totalFloors, propertyType, true, heatingType, condition).Value;
            
            var buyer = Domain.Domain.Customers.Buyer.Buyer.Create(clientIdObj, searchCriteria).Value;
            
            _mockBuyerService.Setup(x => x.GetBuyerByIdAsync(buyerId))
                .ReturnsAsync(Result.Success(buyer));

            // Act
            var actionResult = await _controller.DeleteBuyer(buyerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var buyerDto = Assert.IsType<BuyerDto>(okResult.Value);
            // Проверяем, что возвращаемые данные соответствуют ожидаемым
            Assert.Equal(clientId, buyerDto.ClientId);
        }
    }
}
