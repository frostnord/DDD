using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий электронную почту
    /// </summary>
    public class Email : IEquatable<Email>
    {
        /// <summary>
        /// Значение электронной почты
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создает новый экземпляр электронной почты
        /// </summary>
        /// <param name="value">Электронная почта</param>
        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра электронной почты с возвратом результата
        /// </summary>
        /// <param name="value">Электронная почта</param>
        /// <returns>Result с экземпляром Email при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Email> Create(string value)
        {
            // Проверяем, что значение не является null, пустой строкой или строкой с одними пробелами
            // Это необходимо, чтобы избежать исключения при обработке регулярного выражения
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<Email>("Email не может быть пустым");
            }

            var trimmedValue = value.Trim();

            // Проверяем формат email с помощью регулярного выражения
            if (!IsValidEmail(trimmedValue))
            {
                return Result.Failure<Email>("Некорректный формат email");
            }

            return Result.Success(new Email(trimmedValue));
        }

        private static bool IsValidEmail(string email)
        {
            // Регулярное выражение для проверки формата email
            var regex = new System.Text.RegularExpressions.Regex(
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled|RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        public override bool Equals(object obj)
        {
            if (obj is Email other)
            {
                return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public bool Equals(Email other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value?.ToLowerInvariant().GetHashCode() ?? 0;
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
    }
}