using CSharpFunctionalExtensions;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;

namespace Domain.Customers.Buyer
{
    /// <summary>
    /// Покупатель (отдельная сущность от Клиента/Продавца)
    /// </summary>
    public class BuyerEntity : Entity<BuyerId>
    {
        public const int MAX_HEATING_TYPE_LENGTH = 20;
        public ClientId ClientId { get; private set; }
        public ClientSearchCriteria SearchCriteria { get; private set; }

        protected BuyerEntity(BuyerId id, ClientId clientId, ClientSearchCriteria searchCriteria)
            : base(id)
        {
            ClientId = clientId;
            SearchCriteria = searchCriteria;
        }
        
        // EF Core конструктор
        protected BuyerEntity()
        {
        }
        
        public static Result<BuyerEntity> Create(ClientId clientId, ClientSearchCriteria searchCriteria)
        {
            var errors = new List<string>();
            if (clientId == null) errors.Add("Клиент не может быть пустым");
            else if (clientId.Value == Guid.Empty) errors.Add("Идентификатор клиента не может быть пустым");
            if (searchCriteria == null) errors.Add("Критерии поиска не могут быть пустыми");

            if (errors.Count > 0)
                return Result.Failure<BuyerEntity>(string.Join("; ", errors));

            var id = BuyerId.Create(Guid.NewGuid()).Value;
            return Result.Success(new BuyerEntity(id, clientId, searchCriteria));
        }

        public void UpdateSearchCriteria(ClientSearchCriteria newSearchCriteria)
        {
            SearchCriteria = newSearchCriteria;
        }
    }
}