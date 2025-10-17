using System;
using DDD.Domain;
using DDD.Domain.ValueObjects;

namespace Domain.Tests
{
    public static class SellerTest
    {
        public static void RunSellerTests()
        {
            Console.WriteLine("\n\n=== Тестирование сущности Seller ===\n");

            // Arrange: ClientId
            var clientId = ClientId.Create(Guid.NewGuid()).Value;

            // Act
            var sellerResult = Seller.Create(clientId);

            // Assert
            if (sellerResult.IsFailure)
            {
                Console.WriteLine($"✗ Ошибка создания Seller: {sellerResult.Error}");
            }
            else
            {
                var seller = sellerResult.Value;
                Console.WriteLine("✓ Seller успешно создан");
                Console.WriteLine($"  SellerId: {seller.Id.Value}");
                Console.WriteLine($"  ClientId: {seller.ClientId.Value}");

                // Добавление PropertyId
                var propertyId = PropertyId.Create(Guid.NewGuid()).Value;
                seller.AddProperty(propertyId);
                Console.WriteLine($"  Добавлен PropertyId: {propertyId.Value}");
            }

            // Negative: пустой ClientId
            var emptyClientId = ClientId.Create(Guid.Empty);
            if (emptyClientId.IsFailure)
            {
                Console.WriteLine("✓ Ожидаемая ошибка при пустом ClientId (валидатор TypedId)");
            }
            else
            {
                var badSeller = Seller.Create(emptyClientId.Value);
                Console.WriteLine(badSeller.IsFailure
                    ? $"✓ Ожидаемая ошибка создания Seller: {badSeller.Error}"
                    : "✗ Ожидалась ошибка при создании Seller с пустым ClientId");
            }
        }
    }
}
