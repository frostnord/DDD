using System;
using CSharpFunctionalExtensions;
using DDD.Domain;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;

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

            // Создание PropertyDetails (правильный подход DDD)
            var detailsResult = PropertyDetails.Create(
                85,                      // площадь
                3,                       // комнат
                5,                       // этаж
                9,                       // всего этажей
                PropertyType.Apartment,
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

            // Создание записи о владельце
            var ownerRecordResult = OwnershipRecord.Create("Иванов Иван Иванович", DateTime.Now, "Покупка");
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
            Console.WriteLine($"\nТекущий владелец: {property.GetCurrentOwner().OwnerName}");
            
            // Демонстрация работы с историей владения
            Console.WriteLine($"\nВсего записей в истории владения: {property.OwnershipHistory.Count}");
            
            // Демонстрация работы с PropertyDetails и Value Objects
            Console.WriteLine($"\n=== Детали недвижимости (Value Objects) ===");
            Console.WriteLine($"Тип: {property.Details.Type.GetDisplayName()}");
            Console.WriteLine($"Площадь: {property.Details.Area}");
            Console.WriteLine($"Количество комнат: {property.Details.NumberOfRooms}");
            Console.WriteLine($"Этаж: {property.Details.Floor} из {property.Details.TotalFloors}");
            Console.WriteLine($"Балкон: {(property.Details.HasBalcony ? "Да" : "Нет")}");
            Console.WriteLine($"Парковка: {(property.Details.HasParking ? "Да" : "Нет")}");
            Console.WriteLine($"Отопление: {property.Details.HeatingType}");
            Console.WriteLine($"Состояние: {property.Details.Condition}");
            Console.WriteLine($"Средняя площадь комнаты: {property.Details.GetRoomArea()} кв.м");
            
            // Демонстрация доступа к внутренним значениям Value Objects
            Console.WriteLine($"\n=== Внутренние значения Value Objects ===");
            Console.WriteLine($"NumberOfRooms.Value: {property.Details.NumberOfRooms.Value}");
            Console.WriteLine($"Floor.Value: {property.Details.Floor.Value}");
            Console.WriteLine($"TotalFloors.Value: {property.Details.TotalFloors.Value}");
            Console.WriteLine($"HeatingType.Value: {property.Details.HeatingType.Value}");
            Console.WriteLine($"Condition.Value: {property.Details.Condition.Value}");
            
            // Добавление нового владельца
            var newOwnerResult = OwnershipRecord.Create("Петров Петр Петрович", DateTime.Now.AddYears(1), "Покупка");
            if (newOwnerResult.IsSuccess)
            {
                property.AddOwnershipRecord(newOwnerResult.Value);
                Console.WriteLine($"\nНовый владелец добавлен. Текущий владелец: {property.GetCurrentOwner().OwnerName}");
            }
            
            // Тестирование валидации
            Console.WriteLine($"\n=== Тестирование валидации ===");
            
            var invalidDetailsResult = PropertyDetails.Create(
                -10,     // ❌ Отрицательная площадь
                -5,      // ❌ Отрицательное количество комнат
                15,      // ❌ Этаж больше общего количества
                10,      // Всего этажей
                PropertyType.Apartment
            );

            if (invalidDetailsResult.IsFailure)
            {
                Console.WriteLine($"✓ Валидация сработала корректно:");
                Console.WriteLine($"  Ошибки: {invalidDetailsResult.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация не сработала!");
            }
            
            // Тестирование подвального этажа
            Console.WriteLine($"\n=== Тестирование подвального этажа ===");
            
            var basementDetailsResult = PropertyDetails.Create(
                50,      // площадь
                2,       // комнат
                -1,      // ❗ Подвальный этаж
                5,       // всего этажей в здании
                PropertyType.Commercial,
                hasParking: false,
                heatingType: "Автономное",
                condition: "Хорошее"
            );

            if (basementDetailsResult.IsSuccess)
            {
                var basementOwnerResult = OwnershipRecord.Create("Коммерсант Иван Иванович", DateTime.Now, "Покупка");
                
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
                    Console.WriteLine($"✓ Подвальное помещение создано успешно:");
                    Console.WriteLine($"  Этаж: {basement.Details.Floor}");
                    Console.WriteLine($"  Этаж (значение): {basement.Details.Floor.Value}");
                    Console.WriteLine($"  {basement}");
                }
                else
                {
                    Console.WriteLine($"✗ Ошибка создания подвального помещения: {basementPropertyResult.Error}");
                }
            }
            
            Console.WriteLine($"\n=== Тест завершен ===");
        }
    }
}