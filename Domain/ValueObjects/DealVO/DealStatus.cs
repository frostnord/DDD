using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий статус сделки
    /// </summary>
    public class DealStatus : ValueObject
    {
        /// <summary>
        /// Статус: Создана
        /// </summary>
        public static readonly DealStatus Created = new DealStatus("Created", "Создана");
        
        /// <summary>
        /// Статус: Подтверждена
        /// </summary>
        public static readonly DealStatus Confirmed = new DealStatus("Confirmed", "Подтверждена");
        
        /// <summary>
        /// Статус: Завершена
        /// </summary>
        public static readonly DealStatus Completed = new DealStatus("Completed", "Завершена");
        
        /// <summary>
        /// Статус: Отменена
        /// </summary>
        public static readonly DealStatus Cancelled = new DealStatus("Cancelled", "Отменена");

        /// <summary>
        /// Код статуса
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// Отображаемое имя статуса
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Создает новый экземпляр статуса сделки
        /// </summary>
        /// <param name="code">Код статуса</param>
        /// <param name="displayName">Отображаемое имя</param>
        private DealStatus(string code, string displayName)
        {
            Code = code;
            DisplayName = displayName;
        }

        /// <summary>
        /// Получает статус сделки по коду
        /// </summary>
        /// <param name="code">Код статуса</param>
        /// <returns>Статус сделки или null, если не найден</returns>
        public static DealStatus FromCode(string code)
        {
            switch (code?.ToLower())
            {
                case "created":
                    return Created;
                case "confirmed":
                    return Confirmed;
                case "completed":
                    return Completed;
                case "cancelled":
                    return Cancelled;
                default:
                    return null;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}