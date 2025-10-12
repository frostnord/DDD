using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects.PropertyDetailsVO
{
    /// <summary>
    /// Объект значения, представляющий тип отопления
    /// </summary>
    public class HeatingType
    {
        /// <summary>
        /// Значение типа отопления
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создает новый экземпляр типа отопления
        /// </summary>
        /// <param name="value">Тип отопления</param>
        private HeatingType(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра типа отопления с возвратом результата
        /// </summary>
        /// <param name="value">Тип отопления</param>
        /// <returns>Result с экземпляром HeatingType при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<HeatingType> Create(string value)
        {
            // Если не указано, используем значение по умолчанию
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Success(new HeatingType("Не указано"));
            }

            var trimmedValue = value.Trim();

            if (trimmedValue.Length > 100)
            {
                return Result.Failure<HeatingType>("Тип отопления не может превышать 100 символов");
            }

            // Проверка на допустимые значения (можно расширить)
            var validTypes = new[] { "Центральное", "Газовое", "Электрическое", "Автономное", "Печное", "Не указано" };
            var isValid = Array.Exists(validTypes, t => t.Equals(trimmedValue, StringComparison.OrdinalIgnoreCase));

            if (!isValid)
            {
                // Разрешаем любое значение, но выводим предупреждение в комментарии
                // В реальном проекте можно сделать строже
            }

            return Result.Success(new HeatingType(trimmedValue));
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (obj is HeatingType other)
            {
                return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(HeatingType heatingType) => heatingType.Value;
    }
}
