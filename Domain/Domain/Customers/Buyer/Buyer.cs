using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Buyer.VO;
using Domain.Domain.Customers.Client.VO;

namespace Domain.Domain.Customers.Buyer
{
    /// <summary>
    /// Покупатель (отдельная сущность от Клиента/Продавца)
    /// </summary>
    public class Buyer : Entity<BuyerId>
    {
        public const int MAX_HEATING_TYPE_LENGTH = 20;
        public ClientId ClientId { get; private set; }
        public ClientSearchCriteria SearchCriteria { get; private set; }

        private Buyer(BuyerId id, ClientId clientId, ClientSearchCriteria searchCriteria)
            : base(id)
        {
            ClientId = clientId;
            SearchCriteria = searchCriteria;
        }

        public static Result<Buyer> Create(ClientId clientId, ClientSearchCriteria searchCriteria)
        {
            var errors = new List<string>();
            if (clientId == null) errors.Add("Клиент не может быть пустым");
            else if (clientId.Value == Guid.Empty) errors.Add("Идентификатор клиента не может быть пустым");
            if (searchCriteria == null) errors.Add("Критерии поиска не могут быть пустыми");

            if (errors.Count > 0)
                return Result.Failure<Buyer>(string.Join("; ", errors));

            var id = BuyerId.Create(Guid.NewGuid()).Value;
            return Result.Success(new Buyer(id, clientId, searchCriteria));
        }

        public void UpdateSearchCriteria(ClientSearchCriteria newSearchCriteria)
        {
            SearchCriteria = newSearchCriteria;
        }
    }
}