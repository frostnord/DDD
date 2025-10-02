using System;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий статус недвижимости
    /// </summary>
    public class PropertyStatus
    {
        /// <summary>
        /// Текущий статус недвижимости
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Статус "в продаже"
        /// </summary>
        public static readonly PropertyStatus ForSale = new PropertyStatus("в продаже");
        
        /// <summary>
        /// Статус "забронирован"
        /// </summary>
        public static readonly PropertyStatus Reserved = new PropertyStatus("забронирован");
        
        /// <summary>
        /// Статус "продан"
        /// </summary>
        public static readonly PropertyStatus Sold = new PropertyStatus("продан");

        /// <summary>
        /// Создает новый экземпляр статуса недвижимости
        /// </summary>
        /// <param name="status">Статус недвижимости</param>
        private PropertyStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Статус недвижимости не может быть пустым", nameof(status));
            }
            
            Status = status;
        }

        public override string ToString() => Status;

        /// <summary>
        /// Создает статус недвижимости из строки
        /// </summary>
        /// <param name="status">Строка статуса</param>
        /// <returns>Объект PropertyStatus</returns>
        /// <exception cref="ArgumentException">Вызывается, если передан недопустимый статус</exception>
        public static PropertyStatus FromString(string status)
        {
            switch (status?.ToLower())
            {
                case "в продаже":
                    return ForSale;
                case "забронирован":
                    return Reserved;
                case "продан":
                    return Sold;
                default:
                    throw new ArgumentException($"Недопустимый статус недвижимости: {status}", nameof(status));
            }
        }
    }
}