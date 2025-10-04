using System;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.PropertyDetailsVO;

namespace Domain.Tests
{
    public class ClientTest
    {
        public static void RunClientTests()
        {
            Console.WriteLine("\n\n=== Тестирование сущности Client ===\n");

            // Создание контактной информации
            var emailResult = Email.Create("john.doe@example.com");
            if (emailResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании email: {emailResult.Error}");
                return;
            }

            var phoneResult = PhoneNumber.Create("+79123456789");
            if (phoneResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании номера телефона: {phoneResult.Error}");
                return;
            }

            var contactInfoResult = ContactInfo.Create(emailResult.Value, phoneResult.Value);
            if (contactInfoResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании контактной информации: {contactInfoResult.Error}");
                return;
            }

            // Создание имени
            var firstNameResult = Name.Create("Иван");
            if (firstNameResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании имени: {firstNameResult.Error}");
                return;
            }

            var lastNameResult = Name.Create("Иванов");
            if (lastNameResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании фамилии: {lastNameResult.Error}");
                return;
            }

            // Создание критериев поиска
            var areaResult = Area.Create(75);
            var roomsResult = NumberOfRooms.Create(2);
            var floorResult = Floor.Create(3);
            var totalFloorsResult = TotalFloors.Create(9);
            var heatingTypeResult = HeatingType.Create("Центральное");
            var conditionResult = PropertyCondition.Create("Евроремонт");

            if (areaResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании площади: {areaResult.Error}");
                return;
            }

            if (roomsResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании количества комнат: {roomsResult.Error}");
                return;
            }

            if (floorResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании этажа: {floorResult.Error}");
                return;
            }

            if (totalFloorsResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании общего количества этажей: {totalFloorsResult.Error}");
                return;
            }

            if (heatingTypeResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании типа отопления: {heatingTypeResult.Error}");
                return;
            }

            if (conditionResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании состояния: {conditionResult.Error}");
                return;
            }

            var searchCriteriaResult = ClientSearchCriteria.Create(
                areaResult.Value,
                roomsResult.Value,
                floorResult.Value,
                totalFloorsResult.Value,
                DDD.Domain.ValueObjects.PropertyType.Apartment,
                preferBalcony: true,
                preferParking: true,
                heatingTypeResult.Value,
                conditionResult.Value
            );

            if (searchCriteriaResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании критериев поиска: {searchCriteriaResult.Error}");
                return;
            }

            // Создание клиента
            var clientResult = Client.Create(
                firstNameResult.Value,
                lastNameResult.Value,
                contactInfoResult.Value,
                searchCriteriaResult.Value
            );

            if (clientResult.IsFailure)
            {
                Console.WriteLine($"Ошибка при создании клиента: {clientResult.Error}");
                return;
            }

            var client = clientResult.Value;
            Console.WriteLine($"Клиент создан успешно:");
            Console.WriteLine($"  ID: {client.Id}");
            Console.WriteLine($"  Имя: {client.FirstName}");
            Console.WriteLine($"  Фамилия: {client.LastName}");
            Console.WriteLine($"  Контактная информация: {client.ContactInfo}");
            Console.WriteLine($"  Полное имя: {client.GetFullName()}");
            Console.WriteLine($"  Дата создания: {client.CreatedAt}");
            Console.WriteLine($"  Критерии поиска: {client.SearchCriteria}");

            // Демонстрация обновления контактной информации
            Console.WriteLine("\n=== Обновление контактной информации ===");
            var newEmailResult = Email.Create("ivan.ivanov@newemail.com");
            var newPhoneResult = PhoneNumber.Create("+79876543210");
            var newContactInfoResult = ContactInfo.Create(newEmailResult.Value, newPhoneResult.Value);

            if (newContactInfoResult.IsSuccess)
            {
                client.UpdateContactInfo(newContactInfoResult.Value);
                Console.WriteLine($"Контактная информация обновлена: {client.ContactInfo}");
                Console.WriteLine($"Дата обновления: {client.UpdatedAt}");
            }

            // Демонстрация обновления критериев поиска
            Console.WriteLine("\n=== Обновление критериев поиска ===");
            var newSearchCriteriaResult = ClientSearchCriteria.Create(
                areaResult.Value,
                roomsResult.Value,
                floorResult.Value,
                totalFloorsResult.Value,
                DDD.Domain.ValueObjects.PropertyType.House
            );

            if (newSearchCriteriaResult.IsSuccess)
            {
                client.UpdateSearchCriteria(newSearchCriteriaResult.Value);
                Console.WriteLine($"Критерии поиска обновлены: {client.SearchCriteria}");
                Console.WriteLine($"Дата обновления: {client.UpdatedAt}");
            }

            // Демонстрация работы с бронированиями
            Console.WriteLine("\n=== Работа с бронированиями ===");
            var bookingId = Guid.NewGuid();
            client.AddBookingId(bookingId);
            Console.WriteLine($"Добавлено бронирование: {bookingId}");
            Console.WriteLine($"Количество бронирований: {client.BookingIds.Count}");

            var bookingId2 = Guid.NewGuid();
            client.AddBookingId(bookingId2);
            Console.WriteLine($"Добавлено еще одно бронирование: {bookingId2}");
            Console.WriteLine($"Количество бронирований: {client.BookingIds.Count}");

            client.RemoveBookingId(bookingId);
            Console.WriteLine($"Удалено бронирование: {bookingId}");
            Console.WriteLine($"Количество бронирований: {client.BookingIds.Count}");

            // Тестирование валидации
            Console.WriteLine("\n=== Тестирование валидации ===");

            // Попытка создать клиента без имени
            var invalidClientResult = Client.Create(
                null, // ❌ Пустое имя
                lastNameResult.Value,
                contactInfoResult.Value
            );

            if (invalidClientResult.IsFailure)
            {
                Console.WriteLine($"✓ Валидация сработала корректно при создании клиента без имени:");
                Console.WriteLine($"  Ошибки: {invalidClientResult.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация не сработала при создании клиента без имени!");
            }

            // Попытка создать клиента без фамилии
            var invalidClientResult2 = Client.Create(
                firstNameResult.Value,
                null, // ❌ Пустая фамилия
                contactInfoResult.Value
            );

            if (invalidClientResult2.IsFailure)
            {
                Console.WriteLine($"✓ Валидация сработала корректно при создании клиента без фамилии:");
                Console.WriteLine($"  Ошибки: {invalidClientResult2.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация не сработала при создании клиента без фамилии!");
            }

            // Попытка создать клиента без контактной информации
            var invalidClientResult3 = Client.Create(
                firstNameResult.Value,
                lastNameResult.Value,
                null // ❌ Пустая контактная информация
            );

            if (invalidClientResult3.IsFailure)
            {
                Console.WriteLine($"✓ Валидация сработала корректно при создании клиента без контактной информации:");
                Console.WriteLine($"  Ошибки: {invalidClientResult3.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация не сработала при создании клиента без контактной информации!");
            }

            // Тестирование Value Objects
            Console.WriteLine("\n=== Тестирование Value Objects ===");

            // Тестирование Email
            var invalidEmailResult = Email.Create("invalid-email");
            if (invalidEmailResult.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email сработала корректно:");
                Console.WriteLine($"  Ошибки: {invalidEmailResult.Error}");
            }

            // Тестирование PhoneNumber
            var invalidPhoneResult = PhoneNumber.Create("invalid-phone");
            if (invalidPhoneResult.IsFailure)
            {
                Console.WriteLine($"✓ Валидация номера телефона сработала корректно:");
                Console.WriteLine($"  Ошибки: {invalidPhoneResult.Error}");
            }

            // Тестирование Name
            var invalidNameResult = Name.Create("A"); // Слишком короткое имя
            if (invalidNameResult.IsFailure)
            {
                Console.WriteLine($"✓ Валидация имени сработала корректно:");
                Console.WriteLine($"  Ошибки: {invalidNameResult.Error}");
            }

            // Тестирование равенства сущностей
            Console.WriteLine("\n=== Тестирование равенства сущностей ===");
            var client2Result = Client.Create(
                firstNameResult.Value,
                lastNameResult.Value,
                contactInfoResult.Value
            );

            if (client2Result.IsSuccess)
            {
                Console.WriteLine($"Сравнение разных клиентов с одинаковыми данными: {client.Equals(client2Result.Value)}");
                Console.WriteLine($"Сравнение клиента с самим собой: {client.Equals(client)}");
                Console.WriteLine($"Сравнение с null: {client.Equals(null)}");
                Console.WriteLine($"Сравнение с другим объектом: {client.Equals("not a client")}");
                Console.WriteLine($"Hash код клиента: {client.GetHashCode()}");
            }

            Console.WriteLine("\n=== Тестирование клиента завершено ===");
        }
    }
}