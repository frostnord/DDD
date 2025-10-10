using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.ClientVO;

namespace DDD.Domain.Entities.Deal
{
    /// <summary>
    /// Сущность совершенной сделки клиента
    /// </summary>
    public class CompletedDeal : CSharpFunctionalExtensions.Entity<CompletedDealId>
    {
        
        /// <summary>
        /// Идентификатор клиента, участвовавшего в сделке
        /// </summary>
        public ClientId ClientId { get; private set; }
        
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

        private CompletedDeal(CompletedDealId id, ClientId clientId, PropertyId propertyId, DateTime dealDate, Price dealAmount, string dealType)
            : base(id)
        {
            ClientId = clientId;
            PropertyId = propertyId;
            DealDate = dealDate;
            DealAmount = dealAmount;
            DealType = dealType;
            CreatedAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Создает новый экземпляр совершенной сделки через фабричный метод
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="dealDate">Дата совершения сделки</param>
        /// <param name="dealAmount">Сумма сделки</param>
        /// <param name="dealType">Тип сделки</param>
        /// <returns>Результат с совершенной сделкой или ошибкой</returns>
        public static Result<CompletedDeal> Create(ClientId clientId, PropertyId propertyId, DateTime dealDate, Price dealAmount, string dealType)
        {
            var validationErrors = new List<string>();

            if (clientId == null || clientId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор клиента не может быть пустым");

            if (propertyId == null || propertyId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

            if (dealAmount == null)
                validationErrors.Add("Сумма сделки не может быть пустой");

            if (string.IsNullOrWhiteSpace(dealType))
                validationErrors.Add("Тип сделки не может быть пустым");

            if (dealDate > DateTime.UtcNow)
                validationErrors.Add("Дата сделки не может быть в будущем");
            
            var id = CompletedDealId.New();

            if (validationErrors.Count > 0)
            {
                return Result.Failure<CompletedDeal>(string.Join("; ", validationErrors));
            }

            var deal = new CompletedDeal(id, clientId, propertyId, dealDate, dealAmount, dealType);
            return Result.Success(deal);
        }
    }
}