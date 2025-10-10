using System;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Перечисление, представляющее статус недвижимости
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>
        /// Статус "в продаже"
        /// </summary>
        ForSale,
        
        /// <summary>
        /// Статус "забронирован"
        /// </summary>
        Reserved,
        
        /// <summary>
        /// Статус "продан"
        /// </summary>
        Sold
    }

    /// <summary>
    /// Расширения для PropertyStatus
    /// </summary>
    public static class PropertyStatusExtensions
    {
        /// <summary>
        /// Получает строковое представление статуса недвижимости
        /// </summary>
        /// <param name="status">Статус недвижимости</param>
        /// <returns>Строковое представление статуса</returns>
        public static string GetDisplayName(this PropertyStatus status)
        {
            switch (status)
            {
                case PropertyStatus.ForSale:
                    return "в продаже";
                case PropertyStatus.Reserved:
                    return "забронирован";
                case PropertyStatus.Sold:
                    return "продан";
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}