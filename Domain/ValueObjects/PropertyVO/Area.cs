using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий площадь недвижимости
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Значение площади в квадратных метрах
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Создает новый экземпляр площади
        /// </summary>
        /// <param name="value">Площадь в квадратных метрах</param>
        private Area(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра площади с возвратом результата
        /// </summary>
        /// <param name="value">Площадь в квадратных метрах</param>
        /// <returns>Result с экземпляром Area при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Area> Create(int value)
        {
            var errors = new List<string>();

            if (value <= 0)
                errors.Add("Площадь должна быть положительной");

            if (value > 1000000)
                errors.Add("Площадь не может превышать 1000000 кв. м");

            return errors.Count > 0
                ? Result.Failure<Area>(string.Join("; ", errors))
                : Result.Success(new Area(value));
        }

        public override string ToString()
        {
            return $"{Value} м²";
        }

        public override bool Equals(object obj)
        {
            if (obj is Area other)
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