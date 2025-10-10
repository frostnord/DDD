using System;
using CSharpFunctionalExtensions;
using Domain.Tests;

namespace DDD.Domain.ValueObjects
{
    public class EmailTest
    {
        public static void RunEmailTests()
        {
            Console.WriteLine("\n\n=== Тестирование Value Object Email ===\n");

            // Тест 1: Создание корректного email
            var validEmail = "test@example.com";
            var result = Email.Create(validEmail);
            if (result.IsSuccess)
            {
                Console.WriteLine($"✓ Создание корректного email '{validEmail}' прошло успешно: {result.Value.Value}");
            }
            else
            {
                Console.WriteLine($"✗ Создание корректного email '{validEmail}' завершилось с ошибкой: {result.Error}");
            }

            // Тест 2: Создание email с поддоменом
            var emailWithSubdomain = "user@subdomain.example.com";
            var result2 = Email.Create(emailWithSubdomain);
            if (result2.IsSuccess)
            {
                Console.WriteLine($"✓ Создание email с поддоменом '{emailWithSubdomain}' прошло успешно: {result2.Value.Value}");
            }
            else
            {
                Console.WriteLine($"✗ Создание email с поддоменом '{emailWithSubdomain}' завершилось с ошибкой: {result2.Error}");
            }

            // Тест 3: Создание email с цифрами
            var emailWithNumbers = "user123@example123.com";
            var result3 = Email.Create(emailWithNumbers);
            if (result3.IsSuccess)
            {
                Console.WriteLine($"✓ Создание email с цифрами '{emailWithNumbers}' прошло успешно: {result3.Value.Value}");
            }
            else
            {
                Console.WriteLine($"✗ Создание email с цифрами '{emailWithNumbers}' завершилось с ошибкой: {result3.Error}");
            }

            // Тест 4: Создание email со специальными символами
            var emailWithSpecialChars = "user.name+tag@example.com";
            var result4 = Email.Create(emailWithSpecialChars);
            if (result4.IsSuccess)
            {
                Console.WriteLine($"✓ Создание email со специальными символами '{emailWithSpecialChars}' прошло успешно: {result4.Value.Value}");
            }
            else
            {
                Console.WriteLine($"✗ Создание email со специальными символами '{emailWithSpecialChars}' завершилось с ошибкой: {result4.Error}");
            }

            // Тест 5: Создание пустого email
            var emptyEmail = "";
            var result5 = Email.Create(emptyEmail);
            if (result5.IsFailure)
            {
                Console.WriteLine($"✓ Валидация пустого email сработала корректно: {result5.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация пустого email не сработала! Email создан: {result5.Value.Value}");
            }

            // Тест 6: Создание null email
            string nullEmail = null;
            var result6 = Email.Create(nullEmail);
            if (result6.IsFailure)
            {
                Console.WriteLine($"✓ Валидация null email сработала корректно: {result6.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация null email не сработала! Email создан: {result6.Value.Value}");
            }

            // Тест 7: Создание email только с пробелами
            var whitespaceEmail = "   ";
            var result7 = Email.Create(whitespaceEmail);
            if (result7.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email с пробелами сработала корректно: {result7.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация email с пробелами не сработала! Email создан: {result7.Value.Value}");
            }

            // Тест 8: Создание email без символа @
            var invalidEmailWithoutAt = "invalid-email.com";
            var result8 = Email.Create(invalidEmailWithoutAt);
            if (result8.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email без символа @ сработала корректно: {result8.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация email без символа @ не сработала! Email создан: {result8.Value.Value}");
            }

            // Тест 9: Создание email без домена
            var invalidEmailWithoutDomain = "user@";
            var result9 = Email.Create(invalidEmailWithoutDomain);
            if (result9.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email без домена сработала корректно: {result9.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация email без домена не сработала! Email создан: {result9.Value.Value}");
            }

            // Тест 10: Создание email без домена верхнего уровня
            var invalidEmailWithoutTLD = "user@example";
            var result10 = Email.Create(invalidEmailWithoutTLD);
            if (result10.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email без домена верхнего уровня сработала корректно: {result10.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация email без домена верхнего уровня не сработала! Email создан: {result10.Value.Value}");
            }

            // Тест 11: Создание email с недопустимыми символами
            var invalidEmailWithSpecialChars = "user@exa#mple.com";
            var result11 = Email.Create(invalidEmailWithSpecialChars);
            if (result11.IsFailure)
            {
                Console.WriteLine($"✓ Валидация email с недопустимыми символами сработала корректно: {result11.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Валидация email с недопустимыми символами не сработала! Email создан: {result11.Value.Value}");
            }

            // Тест 12: Создание email с пробелами в начале и конце (должно быть обрезано)
            var emailWithSpaces = " user@example.com ";
            var expectedEmail = "user@example.com";
            var result12 = Email.Create(emailWithSpaces);
            if (result12.IsSuccess && result12.Value.Value == expectedEmail)
            {
                Console.WriteLine($"✓ Email с пробелами в начале и конце был корректно обрезан: '{result12.Value.Value}'");
            }
            else if (result12.IsFailure)
            {
                Console.WriteLine($"✗ Email с пробелами в начале и конце не прошел валидацию: {result12.Error}");
            }
            else
            {
                Console.WriteLine($"✗ Email с пробелами в начале и конце не был корректно обрезан: ожидаем '{expectedEmail}', получили '{result12.Value.Value}'");
            }

            // Тест 13: Создание email с несколькими уровнями домена
            var emailWithMultipleDots = "user@example.co.uk";
            var result13 = Email.Create(emailWithMultipleDots);
            if (result13.IsSuccess)
            {
                Console.WriteLine($"✓ Создание email с несколькими уровнями домена '{emailWithMultipleDots}' прошло успешно: {result13.Value.Value}");
            }
            else
            {
                Console.WriteLine($"✗ Создание email с несколькими уровнями домена '{emailWithMultipleDots}' завершилось с ошибкой: {result13.Error}");
            }

            Console.WriteLine("\n=== Тестирование Email завершено ===");
        }
    }
}