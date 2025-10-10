using System;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// Сущность совершенной сделки клиента
    /// </summary>
    public class CompletedDeal
    {
        /// <summary>
        /// Уникальный идентификатор сделки
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Идентификатор клиента, участвовавшего в сделке
        /// </summary>
        public Guid ClientId { get; private set; }
        
        /// <summary>
        /// Идентификатор объекта недвижимости, участвовавшего в сделке
        /// </summary>
        public Guid PropertyId { get; private set; }
        
        /// <summary>
        /// Дата совершения сделки
        /// </summary>
        public DateTime DealDate { get; private set; }
        
        /// <summary>
        /// Сумма сделки
        /// </summary>
        public Price DealAmount { get; private set; }
        
        /// <summary>
        /// Тип сделки (покупка, аренда и т.д.)
        /// </summary>
        public string DealType { get; private set; }
        
        /// <summary>
        /// Дата создания записи о сделке
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления записи о сделке
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр совершенной сделки через фабричный метод
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="dealDate">Дата совершения сделки</param>
        /// <param name="dealAmount">Сумма сделки</param>
        /// <param name="dealType">Тип сделки</param>
        /// <returns>Результат с совершенной сделкой или ошибкой</returns>
        public static Result<CompletedDeal> Create(Guid clientId, Guid propertyId, DateTime dealDate, Price dealAmount, string dealType)
        {
            var validationErrors = new System.Collections.Generic.List<string>();

            if (clientId == Guid.Empty)
                validationErrors.Add("Идентификатор клиента не может быть пустым");

            if (propertyId == Guid.Empty)
                validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

            if (dealAmount == null)
                validationErrors.Add("Сумма сделки не может быть пустой");

            if (string.IsNullOrWhiteSpace(dealType))
                validationErrors.Add("Тип сделки не может быть пустым");

            if (dealDate > DateTime.UtcNow)
                validationErrors.Add("Дата сделки не может быть в будущем");

            if (validationErrors.Count > 0)
            {
                return Result.Failure<CompletedDeal>(string.Join("; ", validationErrors));
            }

            var deal = new CompletedDeal(clientId, propertyId, dealDate, dealAmount, dealType);
            return Result.Success(deal);
        }

        /// <summary>
        /// Создает новый экземпляр совершенной сделки
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="dealDate">Дата совершения сделки</param>
        /// <param name="dealAmount">Сумма сделки</param>
        /// <param name="dealType">Тип сделки</param>
        private CompletedDeal(Guid clientId, Guid propertyId, DateTime dealDate, Price dealAmount, string dealType)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
            PropertyId = propertyId;
            DealDate = dealDate;
            DealAmount = dealAmount;
            DealType = dealType;
            CreatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            if (obj is CompletedDeal other)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}