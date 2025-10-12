using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий детали сделки
    /// </summary>
    public class DealDetails : ValueObject
    {
        /// <summary>
        /// Дата сделки
        /// </summary>
        public DateTime DealDate { get; }
        
        /// <summary>
        /// Сумма сделки
        /// </summary>
        public Price DealAmount { get; }
        
        /// <summary>
        /// Тип сделки (покупка, аренда и т.д.)
        /// </summary>
        public string DealType { get; }
        
        /// <summary>
        /// Комментарии к сделке
        /// </summary>
        public string Comments { get; }

        /// <summary>
        /// Создает новый экземпляр деталей сделки
        /// </summary>
        /// <param name="dealDate">Дата сделки</param>
        /// <param name="dealAmount">Сумма сделки</param>
        /// <param name="dealType">Тип сделки</param>
        /// <param name="comments">Комментарии к сделке</param>
        private DealDetails(DateTime dealDate, Price dealAmount, string dealType, string comments)
        {
            DealDate = dealDate;
            DealAmount = dealAmount;
            DealType = dealType;
            Comments = comments;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра деталей сделки с возвратом результата
        /// </summary>
        /// <param name="dealDate">Дата сделки</param>
        /// <param name="dealAmount">Сумма сделки</param>
        /// <param name="dealType">Тип сделки</param>
        /// <param name="comments">Комментарии к сделке</param>
        /// <returns>Result с экземпляром DealDetails при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<DealDetails> Create(DateTime dealDate, Price dealAmount, string dealType, string comments = null)
        {
            var validationErrors = new List<string>();

            if (dealAmount == null)
                validationErrors.Add("Сумма сделки не может быть пустой");

            if (string.IsNullOrWhiteSpace(dealType))
                validationErrors.Add("Тип сделки не может быть пустым");

            if (dealDate > DateTime.UtcNow)
                validationErrors.Add("Дата сделки не может быть в будущем");

            return validationErrors.Count > 0
                ? Result.Failure<DealDetails>(string.Join("; ", validationErrors))
                : Result.Success(new DealDetails(dealDate, dealAmount, dealType, comments));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DealDate;
            yield return DealAmount;
            yield return DealType;
            yield return Comments ?? string.Empty;
        }

        public override string ToString()
        {
            return $"Сделка от {DealDate:dd.MM.yyyy}: {DealAmount} ({DealType})";
        }
    }
}