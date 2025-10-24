using System;
using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Property;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace Domain.Tests
{
    public class PropertyTest
    {
        public static void RunPropertyTests()
        {
            // Создание адреса
            var addressResult = Address.Create("Ленина 10", "Москва", 494645677, 129903, "Россия");
            if (addressResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании адреса: {addressResult.Error}");
                return;
            }

            // Создание цены
            var priceResult = Price.Create(5000000);
            if (priceResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании цены: {priceResult.Error}");
                return;
            }

            // Создание описания
            var descriptionResult = Description.Create("Просторная трехкомнатная квартира с евроремонтом");
            if (descriptionResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании описания: {descriptionResult.Error}");
                return;
            }

            // Создание PropertyDetails
            var detailsResult = PropertyDetails.Create(
                85,
                3,
                5,
                9,
                SmartPropertyType.Apartment,
                hasBalcony: true,
                hasParking: true,
                heatingType: "Центральное",
                condition: "Евроремонт"
            );
            if (detailsResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании деталей: {detailsResult.Error}");
                return;
            }

            // Создание ClientId владельца и записи о владельце
            var ownerClientId = ClientId.Create(Guid.NewGuid()).Value;
            var ownerRecordResult =
                OwnershipRecord.Create(ownerClientId, DateTime.UtcNow, null);
            if (ownerRecordResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании записи о владельце: {ownerRecordResult.Error}");
                return;
            }

            // Создание недвижимости
            var propertyResult = Property.Create(
                addressResult.Value,
                priceResult.Value,
                descriptionResult.Value,
                detailsResult.Value,
                ownerRecordResult.Value
            );
            if (propertyResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании недвижимости: {propertyResult.Error}");
                return;
            }

            var property = propertyResult.Value;
            Console.WriteLine("\n" + property.ToString());
            Console.WriteLine($"\nТекущий владелец: {property.GetCurrentOwner().OwnerClientId}");

            // Демонстрация работы с историей владения
            Console.WriteLine($"\nВсего записей в истории владения: {property.OwnershipHistory.Count}");

            // Демонстрация работы с PropertyDetails и Value Objects
            Console.WriteLine("\n=== Детали недвижимости (Value Objects) ===");
            Console.WriteLine($"Тип: {property.Details.Type.DisplayName}");
            Console.WriteLine($"Площадь: {property.Details.Area}");
            Console.WriteLine($"Количество комнат: {property.Details.NumberOfRooms}");
            Console.WriteLine($"Этаж: {property.Details.Floor} из {property.Details.TotalFloors}");
            Console.WriteLine($"Балкон: {(property.Details.HasBalcony ? "Да" : "Нет")}");
            Console.WriteLine($"Парковка: {(property.Details.HasParking ? "Да" : "Нет")}");
            Console.WriteLine($"Отопление: {property.Details.HeatingType}");
            Console.WriteLine($"Состояние: {property.Details.Condition}");
            Console.WriteLine($"Средняя площадь комнаты: {property.Details.GetRoomArea()} кв.м");

            // Внутренние значения VO
            Console.WriteLine("\n=== Внутренние значения Value Objects ===");
            Console.WriteLine($"NumberOfRooms.Value: {property.Details.NumberOfRooms.Value}");
            Console.WriteLine($"Floor.Value: {property.Details.Floor.Value}");
            Console.WriteLine($"TotalFloors.Value: {property.Details.TotalFloors.Value}");
            Console.WriteLine($"HeatingType.Value: {property.Details.HeatingType.Value}");
            Console.WriteLine($"Condition.Value: {property.Details.Condition.Value}");

            // Добавление нового владельца
            var newOwnerClientId = ClientId.Create(Guid.NewGuid()).Value;
            var newOwnerRec = OwnershipRecord.Create(newOwnerClientId,
                DateTime.UtcNow.AddYears(1), null);
            if (newOwnerRec.IsSuccess)
            {
                property.AddOwnershipRecord(newOwnerRec.Value);
                Console.WriteLine(
                    $"\nНовый владелец добавлен. Текущий владелец: {property.GetCurrentOwner().OwnerClientId}");
            }

            // Тестирование валидации Details
            Console.WriteLine("\n=== Тестирование валидации ===");
            var invalidDetailsResult = PropertyDetails.Create(
                -10, // площадь
                -5, // комнаты
                15, // этаж > всего этажей
                10,
                SmartPropertyType.Apartment
            );
            if (invalidDetailsResult.IsFailure)
            {
                Console.WriteLine("✓ Валидация сработала корректно:");
                Console.WriteLine($"  Ошибки: {invalidDetailsResult.Error}");
            }
            else
            {
                Console.WriteLine("✗ Валидация не сработала!");
            }

            // Тестирование подвального этажа
            Console.WriteLine("\n=== Тестирование подвального этажа ===");
            var basementDetailsResult = PropertyDetails.Create(
                50,
                2,
                -1, // подвальный этаж
                5,
                SmartPropertyType.Commercial,
                hasParking: false,
                heatingType: "Автономное",
                condition: "Хорошее"
            );
            if (basementDetailsResult.IsSuccess)
            {
                var basementOwnerClientId = ClientId.Create(Guid.NewGuid()).Value;
                var basementOwnerResult = OwnershipRecord.Create(basementOwnerClientId,
                    DateTime.UtcNow, null);

                var basementPropertyResult = Property.Create(
                    addressResult.Value,
                    Price.Create(3000000).Value,
                    Description.Create("Помещение в подвале с отдельным входом").Value,
                    basementDetailsResult.Value,
                    basementOwnerResult.Value
                );

                if (basementPropertyResult.IsSuccess)
                {
                    var basement = basementPropertyResult.Value;
                    Console.WriteLine("✓ Подвальное помещение создано успешно:");
                    Console.WriteLine($"  Этаж: {basement.Details.Floor}");
                    Console.WriteLine($"  Этаж (значение): {basement.Details.Floor.Value}");
                    Console.WriteLine($"  {basement}");
                }
                else
                {
                    Console.WriteLine($"✗ Ошибка создания подвального помещения: {basementPropertyResult.Error}");
                }
            }

            Console.WriteLine("\n=== Тест завершен ===");
        }
    }
}