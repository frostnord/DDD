using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий описание недвижимости
    /// </summary>
    public class Description
    {
        /// <summary>
        /// Текст описания
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создает новый экземпляр описания
        /// </summary>
        /// <param name="value">Текст описания</param>
        private Description(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра описания с возвратом результата
        /// </summary>
        /// <param name="value">Текст описания</param>
        /// <returns>Result с экземпляром Description при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Description> Create(string value)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(value))
                errors.Add("Описание не может быть пустым");

            if (value != null && value.Length > 1000)
                errors.Add("Описание не может превышать 1000 символов");

            return errors.Count > 0
                ? Result.Failure<Description>(string.Join("; ", errors))
                : Result.Success(new Description(value.Trim()));
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Description other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}