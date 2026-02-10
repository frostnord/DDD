using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal.VO;
using Domain.Property.VO;
using Domain.ValueObjects;

namespace Domain.Deal
{
    /// <summary>
    /// Сущность совершенной сделки (без привязки владения к конкретной стороне)
    /// </summary>
    public class CompletedDealEntity : Entity<CompletedDealId>
    {
        /// <summary>
        /// Клиент-покупатель
        /// </summary>
        public ClientId BuyerClientId { get; private set; }

        /// <summary>
        /// Клиент-продавец
        /// </summary>
        public ClientId SellerClientId { get; private set; }

        /// <summary>
        /// Идентификатор объекта недвижимости, участвовавшего в сделке
        /// </summary>
        public PropertyId PropertyId { get; private set; }

        /// <summary>
        /// Дата совершения сделки
        /// </summary>
        public DateTime DealDate { get; private set; }

        /// <summary>
        /// Сумма сделки
        /// </summary>
        public Price DealAmount { get; private set; }

        /// <summary>
        /// Тип сделки (smart-enum)
        /// </summary>
        public DealType DealType { get; private set; }

        /// <summary>
        /// Дата создания записи о сделке
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Дата последнего обновления записи о сделке
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        protected CompletedDealEntity(CompletedDealId id, ClientId buyerClientId, ClientId sellerClientId,
            PropertyId propertyId, DateTime dealDate, Price dealAmount, DealType dealType)
            : base(id)
        {
            BuyerClientId = buyerClientId;
            SellerClientId = sellerClientId;
            PropertyId = propertyId;
            DealDate = dealDate;
            DealAmount = dealAmount;
            DealType = dealType;
            CreatedAt = DateTime.UtcNow;
        }
        
        // EF Core конструктор
        protected CompletedDealEntity()
        {
        }

        /// <summary>
        /// Создает новый экземпляр совершенной сделки через фабричный метод
        /// </summary>
        /// <param name="buyerClientId">Идентификатор покупателя</param>
        /// <param name="sellerClientId">Идентификатор продавца</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="dealDate">Дата совершения сделки</param>
        /// <param name="dealAmount">Сумма сделки</param>
        /// <param name="dealType">Тип сделки</param>
        /// <returns>Результат с совершенной сделкой или ошибкой</returns>
        public static Result<CompletedDealEntity> Create(ClientId buyerClientId, ClientId sellerClientId,
            PropertyId propertyId, DateTime dealDate, Price dealAmount, DealType dealType)
        {
            var validationErrors = new List<string>();

            if (buyerClientId == null || buyerClientId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор покупателя не может быть пустым");

            if (sellerClientId == null || sellerClientId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор продавца не может быть пустым");

            if (buyerClientId?.Value == sellerClientId?.Value)
                validationErrors.Add("Покупатель и продавец не могут совпадать");

            if (propertyId == null || propertyId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

            if (dealAmount == null)
                validationErrors.Add("Сумма сделки не может быть пустой");

            if (dealType == null)
                validationErrors.Add("Тип сделки не может быть пустым");

            if (dealDate > DateTime.UtcNow)
                validationErrors.Add("Дата сделки не может быть в будущем");

            var id = CompletedDealId.Create(Guid.NewGuid()).Value;

            if (validationErrors.Count > 0)
            {
                return Result.Failure<CompletedDealEntity>(string.Join("; ", validationErrors));
            }

            var deal = new CompletedDealEntity(id, buyerClientId, sellerClientId, propertyId, dealDate, dealAmount, dealType);
            return Result.Success(deal);
        }
    }
}