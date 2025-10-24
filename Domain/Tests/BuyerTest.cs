using System;
using Domain.Domain;
using Domain.Domain.Customers.Buyer;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace Domain.Tests
{
    public static class BuyerTest
    {
        public static void RunBuyerTests()
        {
            Console.WriteLine("\n\n=== Тестирование сущности Buyer ===\n");

            // Arrange: ClientId
            var clientId = ClientId.Create(Guid.NewGuid()).Value;

            // Arrange: обязательные критерии поиска
            var rooms = NumberOfRooms.Create(2).Value;
            var floor = Floor.Create(3).Value;
            var totalFloors = TotalFloors.Create(9).Value;
            var heating = HeatingType.Create("Центральное").Value;
            var condition = PropertyCondition.Create("Хорошее").Value;
            var type = SmartPropertyType.FromName("Apartment");

            var criteria = ClientSearchCriteria.Create(
                rooms,
                floor,
                totalFloors,
                type,
                preferParking: true,
                preferredHeatingType: heating,
                preferredCondition: condition
            ).Value;

            // Act
            var buyerResult = Buyer.Create(clientId, criteria);

            // Assert
            if (buyerResult.IsFailure)
            {
                Console.WriteLine($"✗ Ошибка создания Buyer: {buyerResult.Error}");
            }
            else
            {
                var buyer = buyerResult.Value;
                Console.WriteLine("✓ Buyer успешно создан");
                Console.WriteLine($"  BuyerId: {buyer.Id.Value}");
                Console.WriteLine($"  ClientId: {buyer.ClientId.Value}");
                Console.WriteLine($"  Criteria: Rooms={buyer.SearchCriteria.PreferredNumberOfRooms}, Floor={buyer.SearchCriteria.PreferredFloor}, TotalFloors={buyer.SearchCriteria.PreferredTotalFloors}");
            }

            // Negative: неверный инвариант (этаж > всего этажей)
            var badCriteriaResult = ClientSearchCriteria.Create(
                NumberOfRooms.Create(2).Value,
                Floor.Create(10).Value,
                TotalFloors.Create(5).Value,
                SmartPropertyType.FromName("Apartment"),
                preferParking: null,
                preferredHeatingType: HeatingType.Create("Центральное").Value,
                preferredCondition: PropertyCondition.Create("Среднее").Value
            );
            if (badCriteriaResult.IsFailure)
            {
                Console.WriteLine($"✓ Ожидаемая ошибка критериев: {badCriteriaResult.Error}");
            }
            else
            {
                Console.WriteLine("✗ Ожидалась ошибка валидации критериев (этаж > всего этажей)");
            }
        }
    }
}
