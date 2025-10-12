using System;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;

namespace Domain.Tests
{
    public class PhoneNumberTest
    {
        public static void RunPhoneNumberTests()
        {
            Console.WriteLine("\n\n=== Тестирование Value Object PhoneNumber ===\n");

            // Тестирование различных форматов российских номеров телефонов
            var validPhoneFormats = new[]
            {
                "+79123456789",
                "89123456789",
                "+7(912)345-67-89",
                "8(912)345-67-89",
                "+7 912 345 67 89",
                "8 912 345 67 89",
                "+7-912-345-67-89",
                "8-912-345-67-89"
            };
            
            foreach (var phoneFormat in validPhoneFormats)
            {
                var phoneResult = PhoneNumber.Create(phoneFormat);
                if (phoneResult.IsSuccess)
                {
                    Console.WriteLine($"✓ Создание номера телефона в формате '{phoneFormat}' прошло успешно: {phoneResult.Value.Value}");
                }
                else
                {
                    Console.WriteLine($"✗ Создание номера телефона в формате '{phoneFormat}' завершилось с ошибкой: {phoneResult.Error}");
                }
            }
            
            // Тестирование невалидных форматов российских номеров телефонов
            var invalidPhoneFormats = new[]
            {
                "+19123456789", // Неправильный код страны
                "79123456789",  // Отсутствует +
                "+7912345678",   // Недостаточно цифр
                "+791234567890", // Слишком много цифр
                "8912345678",    // Недостаточно цифр
                "891234567890",  // Слишком много цифр
                "+7(912)345-67-8", // Недостаточно цифр
                "+7(912)345-67-890" // Слишком много цифр
            };
            
            foreach (var phoneFormat in invalidPhoneFormats)
            {
                var phoneResult = PhoneNumber.Create(phoneFormat);
                if (phoneResult.IsFailure)
                {
                    Console.WriteLine($"✓ Валидация номера телефона в формате '{phoneFormat}' сработала корректно:");
                    Console.WriteLine($"  Ошибки: {phoneResult.Error}");
                }
                else
                {
                    Console.WriteLine($"✗ Валидация номера телефона в формате '{phoneFormat}' не сработала! Номер создан: {phoneResult.Value.Value}");
                }
            }
            
            // Тестирование равенства Value Objects
            Console.WriteLine("\n=== Тестирование равенства Value Objects ===");
            var phone1Result = PhoneNumber.Create("+79123456789");
            var phone2Result = PhoneNumber.Create("8(912)345-67-89"); // То же число, другой формат
            
            if (phone1Result.IsSuccess && phone2Result.IsSuccess)
            {
                Console.WriteLine($"Сравнение разных номеров с одинаковыми данными: {phone1Result.Value.Equals(phone2Result.Value)}");
                Console.WriteLine($"Сравнение номера с самим собой: {phone1Result.Value.Equals(phone1Result.Value)}");
                Console.WriteLine($"Сравнение с null: {phone1Result.Value.Equals(null)}");
                Console.WriteLine($"Сравнение с другим объектом: {phone1Result.Value.Equals("not a phone")}");
                Console.WriteLine($"Hash код номера: {phone1Result.Value.GetHashCode()}");
            }
            
            Console.WriteLine("\n=== Тестирование PhoneNumber завершено ===");
        }
    }
}