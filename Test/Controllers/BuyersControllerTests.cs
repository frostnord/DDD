using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presenter.Controllers;
using Presenter.DTOs;
using UseCases.Interfaces;
using UseCases.Interfaces.Services;
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
                ClientId = clientId,
                PreferredNumberOfRooms = 2,
                PreferredFloor = 3,
                PreferredTotalFloors = 9,
                PreferredType = "Apartment",
                PreferredHeatingType = "Central",
                PreferredCondition = "Хорошее",
                PreferParking = true
            };

            var clientIdVO = ClientId.Create(clientId).Value;

            var buyer = BuyerEntity.Create(clientIdVO,
                ClientSearchCriteria.Create(
                    NumberOfRooms.Create(request.PreferredNumberOfRooms).Value,
                    Floor.Create(request.PreferredFloor).Value,
                    TotalFloors.Create(request.PreferredTotalFloors).Value,
                    SmartPropertyType.FromName(request.PreferredType),
                    request.PreferParking,
                    HeatingType.Create(request.PreferredHeatingType).Value,
                    PropertyCondition.Create(request.PreferredCondition).Value
                ).Value
            ).Value;

            var result = Result.Success(buyer);
            _mockBuyerService.Setup(x => x.CreateBuyerAsync(request.ClientId,
                    request.PreferredNumberOfRooms, request.PreferredFloor, request.PreferredTotalFloors,
                    request.PreferredType, request.PreferredHeatingType, request.PreferredCondition,
                    request.PreferParking))
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
                ClientId = clientId,
                PreferredNumberOfRooms = 2,
                PreferredFloor = 3,
                PreferredTotalFloors = 9,
                PreferredType = "Apartment",
                PreferredHeatingType = "Central",
                PreferredCondition = "Хорошее",
                PreferParking = true
            };

            var errorResult = Result.Failure<BuyerEntity>("Validation error");
            _mockBuyerService.Setup(x => x.CreateBuyerAsync(request.ClientId,
                    request.PreferredNumberOfRooms, request.PreferredFloor, request.PreferredTotalFloors,
                    request.PreferredType, request.PreferredHeatingType, request.PreferredCondition,
                    request.PreferParking))
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
            var clientId = Guid.NewGuid();

            var clientIdVO = ClientId.Create(clientId).Value;

            var buyer = BuyerEntity.Create(clientIdVO,
                ClientSearchCriteria.Create(
                    NumberOfRooms.Create(2).Value,
                    Floor.Create(3).Value,
                    TotalFloors.Create(9).Value,
                    SmartPropertyType.FromName("Apartment"),
                    true,
                    HeatingType.Create("Central").Value,
                    PropertyCondition.Create("Хорошее").Value
                ).Value
            ).Value;

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
            var errorResult = Result.Failure<BuyerEntity>("Buyer not found");
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

            var clientIdVO = ClientId.Create(request.ClientId).Value;

            var buyer = BuyerEntity.Create(clientIdVO,
                ClientSearchCriteria.Create(
                    NumberOfRooms.Create(request.PreferredNumberOfRooms).Value,
                    Floor.Create(request.PreferredFloor).Value,
                    TotalFloors.Create(request.PreferredTotalFloors).Value,
                    SmartPropertyType.FromName(request.PreferredType),
                    request.PreferParking,
                    HeatingType.Create(request.PreferredHeatingType).Value,
                    PropertyCondition.Create(request.PreferredCondition).Value
                ).Value
            ).Value;

            var result = Result.Success(buyer);
            _mockBuyerService.Setup(x => x.UpdateBuyerAsync(buyerId, request.ClientId,
                    request.PreferredNumberOfRooms, request.PreferredFloor, request.PreferredTotalFloors,
                    request.PreferredType, request.PreferredHeatingType, request.PreferredCondition,
                    request.PreferParking))
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
            var clientId = Guid.NewGuid();

            // Мокаем успешное удаление
            _mockBuyerService.Setup(x => x.DeleteBuyerAsync(buyerId))
                .ReturnsAsync(Result.Success());

            var clientIdVO = ClientId.Create(clientId).Value;

            var buyer = BuyerEntity.Create(clientIdVO,
                ClientSearchCriteria.Create(
                    NumberOfRooms.Create(2).Value,
                    Floor.Create(3).Value,
                    TotalFloors.Create(9).Value,
                    SmartPropertyType.FromName("Apartment"),
                    true,
                    HeatingType.Create("Central").Value,
                    PropertyCondition.Create("Хорошее").Value
                ).Value
            ).Value;

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

        [Fact]
        public async Task GetBuyers_ReturnsOkResultWithListOfBuyers()
        {
            // Arrange
            var buyers = new List<BuyerEntity>
            {
                BuyerEntity.Create(
                    ClientId.Create(Guid.NewGuid()).Value,
                    ClientSearchCriteria.Create(
                        NumberOfRooms.Create(2).Value,
                        Floor.Create(3).Value,
                        TotalFloors.Create(9).Value,
                        SmartPropertyType.FromName("Apartment"),
                        true,
                        HeatingType.Create("Central").Value,
                        PropertyCondition.Create("Хорошее").Value
                    ).Value
                ).Value,
                BuyerEntity.Create(
                    ClientId.Create(Guid.NewGuid()).Value,
                    ClientSearchCriteria.Create(
                        NumberOfRooms.Create(3).Value,
                        Floor.Create(5).Value,
                        TotalFloors.Create(12).Value,
                        SmartPropertyType.FromName("House"),
                        false,
                        HeatingType.Create("Autonomous").Value,
                        PropertyCondition.Create("Отличное").Value
                    ).Value
                ).Value
            }.AsEnumerable();

            var result = Result.Success(buyers);
            _mockBuyerService.Setup(x => x.GetAllBuyersAsync())
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetBuyers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var buyerDtos = Assert.IsAssignableFrom<IEnumerable<BuyerDto>>(okResult.Value);
            Assert.Equal(2, buyerDtos.Count());
        }
    }
}