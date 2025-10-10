using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects.PropertyDetailsVO
{
    /// <summary>
    /// Объект значения, представляющий этаж
    /// </summary>
    public class Floor
    {
        /// <summary>
        /// Номер этажа
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Создает новый экземпляр этажа
        /// </summary>
        /// <param name="value">Номер этажа</param>
        private Floor(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра этажа с возвратом результата
        /// </summary>
        /// <param name="value">Номер этажа (может быть отрицательным для подвалов: -1, -2 и т.д.)</param>
        /// <param name="totalFloors">Общее количество этажей (для валидации)</param>
        /// <returns>Result с экземпляром Floor при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Floor> Create(int value, int totalFloors = int.MaxValue)
        {
            // Разрешаем отрицательные значения для подвальных этажей (-1, -2, и т.д.)
            // и 0 для цокольного этажа
            if (value < -10)
            {
                return Result.Failure<Floor>("Номер этажа не может быть меньше -10 (слишком глубокий подвал)");
            }

            if (value > totalFloors && value > 0)
            {
                return Result.Failure<Floor>($"Номер этажа ({value}) не может быть больше общего количества этажей ({totalFloors})");
            }

            if (value > 200)
            {
                return Result.Failure<Floor>("Номер этажа не может превышать 200");
            }

            return Result.Success(new Floor(value));
        }

        public override string ToString()
        {
            if (Value < 0)
                return $"{Math.Abs(Value)} подвальный этаж";
            if (Value == 0)
                return "Цокольный этаж";
            return $"{Value} этаж";
        }

        public override bool Equals(object obj)
        {
            if (obj is Floor other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator int(Floor floor) => floor.Value;
    }
}
