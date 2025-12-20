using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller.VO;
using Domain.Property.VO;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Customers.Seller
{
    /// <summary>
    /// Продавец (отдельная сущность от Клиента/Покупателя)
    /// </summary>
    public class SellerEntity : Entity<SellerId>
    {
        public ClientId ClientId { get; private set; }
        private readonly List<PropertyId> _ownedProperties = new();
        [NotMapped]
        public IReadOnlyCollection<PropertyId> OwnedProperties => _ownedProperties.AsReadOnly();

        protected SellerEntity(SellerId id, ClientId clientId) : base(id)
        {
            ClientId = clientId;
        }
        
        // EF Core конструктор
        protected SellerEntity()
        {
        }

        public static Result<SellerEntity> Create(ClientId clientId)
        {
            var errors = new List<string>();

            if (clientId == null)
            {
                errors.Add("Клиент не может быть пустым");
            }

            if (errors.Count > 0)
                return Result.Failure<SellerEntity>(string.Join("; ", errors));

            var id = SellerId.Create(Guid.NewGuid()).Value;
            return Result.Success(new SellerEntity(id, clientId));
        }

        /// <summary>
        /// Привязать объект недвижимости к продавцу (проекция; источник истины в Property)
        /// </summary>
        public Result AttachProperty(PropertyId propertyId)
        {
            if (propertyId == null)
                return Result.Failure("PropertyId не может быть пустым");

            if (_ownedProperties.Contains(propertyId))
                return Result.Success(); // идемпотентно

            _ownedProperties.Add(propertyId);
            return Result.Success();
        }

        /// <summary>
        /// Отвязать объект недвижимости от продавца (проекция; источник истины в Property)
        /// </summary>
        public Result DetachProperty(PropertyId propertyId)
        {
            if (propertyId == null)
                return Result.Failure("PropertyId не может быть пустым");

            var removed = _ownedProperties.Remove(propertyId);
            return removed ? Result.Success() : Result.Failure("У продавца нет такого объекта");
        }
        //     }
        //
        //     public string GetFullName() => $"{FirstName} {LastName}";
    }
}