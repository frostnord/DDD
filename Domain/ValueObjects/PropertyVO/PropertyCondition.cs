using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects.PropertyDetailsVO
{
    /// <summary>
    /// Объект значения, представляющий состояние недвижимости
    /// </summary>
    public class PropertyCondition
    {
        /// <summary>
        /// Значение состояния
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Создает новый экземпляр состояния недвижимости
        /// </summary>
        /// <param name="value">Состояние</param>
        private PropertyCondition(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра состояния недвижимости с возвратом результата
        /// </summary>
        /// <param name="value">Состояние</param>
        /// <returns>Result с экземпляром PropertyCondition при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<PropertyCondition> Create(string value)
        {
            // Если не указано, используем значение по умолчанию
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Success(new PropertyCondition("Не указано"));
            }

            var trimmedValue = value.Trim();

            if (trimmedValue.Length > 100)
            {
                return Result.Failure<PropertyCondition>("Состояние недвижимости не может превышать 100 символов");
            }

            // Проверка на допустимые значения
            var validConditions = new[] 
            { 
                "Новый", 
                "Отличное", 
                "Хорошее", 
                "Удовлетворительное", 
                "Требует ремонта", 
                "Евроремонт", 
                "Косметический ремонт",
                "Черновая отделка",
                "Под ремонт",
                "Не указано" 
            };

            var isValid = Array.Exists(validConditions, c => c.Equals(trimmedValue, StringComparison.OrdinalIgnoreCase));

            if (!isValid)
            {
                // В реальном проекте можно сделать строже или вернуть ошибку
                // Сейчас разрешаем любое значение
            }

            return Result.Success(new PropertyCondition(trimmedValue));
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (obj is PropertyCondition other)
            {
                return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(PropertyCondition condition) => condition.Value;
    }
}
