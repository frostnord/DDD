using System;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий имя (ФИО)
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Полное имя
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создает новый экземпляр имени
        /// </summary>
        /// <param name="value">Полное имя</param>
        private Name(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра имени с возвратом результата
        /// </summary>
        /// <param name="value">Полное имя</param>
        /// <returns>Result с экземпляром Name при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Name> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<Name>("Имя не может быть пустым");
            }

            var trimmedValue = value.Trim();

            if (trimmedValue.Length < 2)
            {
                return Result.Failure<Name>("Имя должно содержать минимум 2 символа");
            }

            if (trimmedValue.Length > 200)
            {
                return Result.Failure<Name>("Имя не может превышать 200 символов");
            }

            // Проверка на допустимые символы (буквы, пробелы, дефисы, точки)
            if (!Regex.IsMatch(trimmedValue, @"^[a-zA-Zа-яА-ЯёЁ\s\-\.]+$"))
            {
                return Result.Failure<Name>("Имя может содержать только буквы, пробелы, дефисы и точки");
            }

            // Проверка на наличие хотя бы одной буквы
            if (!Regex.IsMatch(trimmedValue, @"[a-zA-Zа-яА-ЯёЁ]"))
            {
                return Result.Failure<Name>("Имя должно содержать хотя бы одну букву");
            }

            return Result.Success(new Name(trimmedValue));
        }

        /// <summary>
        /// Получает инициалы (первые буквы слов)
        /// </summary>
        /// <returns>Инициалы</returns>
        public string GetInitials()
        {
            var words = Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var initials = string.Empty;

            foreach (var word in words)
            {
                if (word.Length > 0 && char.IsLetter(word[0]))
                {
                    initials += char.ToUpper(word[0]) + ".";
                }
            }

            return initials.TrimEnd('.');
        }

        /// <summary>
        /// Получает фамилию (первое слово)
        /// </summary>
        /// <returns>Фамилия или пустая строка</returns>
        public string GetLastName()
        {
            var words = Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length > 0 ? words[0] : string.Empty;
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (obj is Name other)
            {
                return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        public static implicit operator string(Name name) => name.Value;
    }
}
