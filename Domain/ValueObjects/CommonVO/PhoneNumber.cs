using System;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий номер телефона
    /// </summary>
    public class PhoneNumber : IEquatable<PhoneNumber>
    {
        /// <summary>
        /// Значение номера телефона
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создает новый экземпляр номера телефона
        /// </summary>
        /// <param name="value">Номер телефона</param>
        private PhoneNumber(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра номера телефона с возвратом результата
        /// </summary>
        /// <param name="value">Номер телефона</param>
        /// <returns>Result с экземпляром PhoneNumber при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<PhoneNumber>("Номер телефона не может быть пустым");
            }

            var cleanedValue = CleanPhoneNumber(value);

            if (!IsValidPhoneNumber(cleanedValue))
            {
                return Result.Failure<PhoneNumber>("Некорректный формат номера телефона");
            }

            return Result.Success(new PhoneNumber(cleanedValue));
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Проверяем, что номер телефона соответствует международному формату
            var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
            return phoneRegex.IsMatch(phoneNumber);
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            // Удаляем все символы, кроме цифр и плюса в начале
            var cleaned = Regex.Replace(phoneNumber, @"[^\d\+]", "");
            // Если плюс в начале отсутствует, добавляем +7 как префикс для России по умолчанию
            if (!cleaned.StartsWith("+"))
            {
                if (cleaned.StartsWith("8")) // Российский формат
                {
                    cleaned = "+7" + cleaned.Substring(1);
                }
                else if (cleaned.Length == 10) // Только 10 цифр
                {
                    cleaned = "+7" + cleaned;
                }
                else if (cleaned.Length == 11 && cleaned.StartsWith("7"))
                {
                    cleaned = "+" + cleaned;
                }
                else if (!cleaned.StartsWith("+"))
                {
                    cleaned = "+" + cleaned;
                }
            }
            return cleaned;
        }

        public override bool Equals(object obj)
        {
            if (obj is PhoneNumber other)
            {
                return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public bool Equals(PhoneNumber other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    }
}