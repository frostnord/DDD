using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects.PropertyDetailsVO
{
    /// <summary>
    /// Объект значения, представляющий количество комнат
    /// </summary>
    public class NumberOfRooms
    {
        /// <summary>
        /// Значение количества комнат
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Создает новый экземпляр количества комнат
        /// </summary>
        /// <param name="value">Количество комнат</param>
        private NumberOfRooms(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра количества комнат с возвратом результата
        /// </summary>
        /// <param name="value">Количество комнат</param>
        /// <returns>Result с экземпляром NumberOfRooms при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<NumberOfRooms> Create(int value)
        {
            if (value < 0)
            {
                return Result.Failure<NumberOfRooms>("Количество комнат не может быть отрицательным");
            }

            if (value > 100)
            {
                return Result.Failure<NumberOfRooms>("Количество комнат не может превышать 100");
            }

            return Result.Success(new NumberOfRooms(value));
        }

        public override string ToString() => $"{Value} комн.";

        public override bool Equals(object obj)
        {
            if (obj is NumberOfRooms other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator int(NumberOfRooms rooms) => rooms.Value;
    }
}
