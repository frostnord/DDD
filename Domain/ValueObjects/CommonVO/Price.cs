using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий цену недвижимости
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Значение цены
        /// </summary>
        public decimal Value { get; private set; }

        /// <summary>
        /// Создает новый экземпляр цены
        /// </summary>
        /// <param name="value">Значение цены</param>
        private Price(decimal value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра цены с возвратом результата
        /// </summary>
        /// <param name="value">Значение цены</param>
        /// <returns>Result с экземпляром Price при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Price> Create(decimal value)
        {
            if (value <= 0)
            {
                return Result.Failure<Price>("Цена не может быть нулевой, отрицательной или отсутствовать.");
            }

            return Result.Success(new Price(value));
        }

        public static implicit operator decimal(Price price) => price.Value;
        public static explicit operator Price(decimal value) => new Price(value);
        public static explicit operator Price(int value) => new Price(value);
        public static explicit operator Price(double value) => new Price((decimal)value);
        
        public override string ToString()
        {
            return Value.ToString("C");
        }
    }
}