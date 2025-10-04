using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects.PropertyDetailsVO
{
    /// <summary>
    /// Объект значения, представляющий общее количество этажей в здании
    /// </summary>
    public class TotalFloors
    {
        /// <summary>
        /// Общее количество этажей
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Создает новый экземпляр общего количества этажей
        /// </summary>
        /// <param name="value">Общее количество этажей</param>
        private TotalFloors(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра общего количества этажей с возвратом результата
        /// </summary>
        /// <param name="value">Общее количество этажей</param>
        /// <returns>Result с экземпляром TotalFloors при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<TotalFloors> Create(int value)
        {
            if (value <= 0)
            {
                return Result.Failure<TotalFloors>("Общее количество этажей должно быть положительным числом");
            }

            if (value > 200)
            {
                return Result.Failure<TotalFloors>("Общее количество этажей не может превышать 200");
            }

            return Result.Success(new TotalFloors(value));
        }

        public override string ToString() => $"{Value} эт.";

        public override bool Equals(object obj)
        {
            if (obj is TotalFloors other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator int(TotalFloors totalFloors) => totalFloors.Value;
    }
}
