using Domain.Domain;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;


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

            // Создание клиента
            var clientResult = Client.Create(
                firstNameResult.Value,
                lastNameResult.Value,
                contactInfoResult.Value
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
            Console.WriteLine($"  Дата регистрации: {client.RegisteredDate}");

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

            // Тестирование Value Objects
            Console.WriteLine("\n=== Тестирование Value Objects ===");

            // Тестирование Email
            var invalidEmailResult = Email.Create("invalid-email");
            if (invalidEmailResult.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email сработала корректно:");
                Console.WriteLine($"  Ошибки: {invalidEmailResult.Error}");
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