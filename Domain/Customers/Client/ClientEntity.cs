using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.ValueObjects;

namespace Domain.Customers.Client
{
    /// <summary>
    /// Сущность клиента в системе управления недвижимостью
    /// </summary>
    public class ClientEntity : Entity<ClientId>
    {
        /// <summary>
        /// Имя клиента
        /// </summary>
        public Name FirstName { get; private set; }

        public Name LastName { get; private set; }
        public ContactInfo ContactInfo { get; private set; }

        public DateTime RegisteredDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <param name="id"></param>
        /// <param name="firstName">Имя клиента</param>
        /// <param name="lastName">Фамилия клиента</param>
        /// <param name="contactInfo">Контактная информация клиента</param>
        protected ClientEntity(ClientId id, Name firstName, Name lastName, ContactInfo contactInfo)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
            RegisteredDate = DateTime.UtcNow;

            // CompletedDeals = new List<CompletedDeal>().AsReadOnly();
            // BookingIds = new List<Guid>().AsReadOnly();
        }
        
        // EF Core конструктор
        protected ClientEntity()
        {
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра клиента с возвратом результата
        /// </summary>
        /// <param name="firstName">Имя клиента</param>
        /// <param name="lastName">Фамилия клиента</param>
        /// <param name="contactInfo">Контактная информация клиента</param>
        /// <returns>Result с экземпляром Client при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<ClientEntity> Create(Name firstName, Name lastName, ContactInfo contactInfo)
        {
            var errors = new List<string>();

            if (firstName == null)
                errors.Add("Имя клиента не может быть пустым");

            if (lastName == null)
                errors.Add("Фамилия клиента не может быть пустой");

            if (contactInfo == null)
                errors.Add("Контактная информация не может быть пустой");

            if (errors.Count > 0)
                return Result.Failure<ClientEntity>(string.Join("; ", errors));


            var id = ClientId.Create(Guid.NewGuid()).Value;
            var client = new ClientEntity(id, firstName, lastName, contactInfo);
            return Result.Success(client);
        }

        /// <summary>
        /// Обновляет контактную информацию клиента
        /// </summary>
        /// <param name="newContactInfo">Новая контактная информация</param>
        public void UpdateContactInfo(ContactInfo newContactInfo)
        {
            if (newContactInfo == null)
            {
                throw new ArgumentNullException(nameof(newContactInfo), "Контактная информация не может быть пустой");
            }

            ContactInfo = newContactInfo;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет данные клиента
        /// </summary>
        /// <param name="firstName">Новое имя</param>
        /// <param name="lastName">Новая фамилия</param>
        /// <param name="contactInfo">Новая контактная информация</param>
        /// <returns>Результат операции обновления</returns>
        public Result UpdateClientData(Name firstName, Name lastName, ContactInfo contactInfo)
        {
            if (firstName == null)
                return Result.Failure("Имя не может быть пустым");

            if (lastName == null)
                return Result.Failure("Фамилия не может быть пустой");

            if (contactInfo == null)
                return Result.Failure("Контактная информация не может быть пустой");

            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        // /// <summary>
        // /// Добавляет совершенную сделку клиенту
        // /// </summary>
        // /// <param name="deal">Совершенная сделка</param>
        // public void AddCompletedDeal(CompletedDeal deal)
        // {
        //     if (deal == null)
        //         throw new ArgumentNullException(nameof(deal), "Сделка не может быть пустой");
        //         
        //     var deals = CompletedDeals.ToList();
        //     if (!deals.Contains(deal))
        //     {
        //         deals.Add(deal);
        //         CompletedDeals = deals.AsReadOnly();
        //         UpdatedAt = DateTime.UtcNow;
        //     }
        // }

        // /// <summary>
        // /// Удаляет совершенную сделку у клиента
        // /// </summary>
        // /// <param name="dealId">Идентификатор сделки</param>
        // public void RemoveCompletedDeal(CompletedDealId dealId)
        // {
        //     var deals = CompletedDeals.ToList();
        //     var dealToRemove = deals.FirstOrDefault(d => d.Id == dealId);
        //     if (dealToRemove != null)
        //     {
        //         deals.Remove(dealToRemove);
        //         CompletedDeals = deals.AsReadOnly();
        //         UpdatedAt = DateTime.UtcNow;
        //     }
        // }
        //
        // /// <summary>
        // /// Добавляет идентификатор бронирования к клиенту
        // /// </summary>
        // /// <param name="bookingId">Идентификатор бронирования</param>
        // public void AddBookingId(Guid bookingId)
        // {
        //     var bookingIds = BookingIds.ToList();
        //     if (!bookingIds.Contains(bookingId))
        //     {
        //         bookingIds.Add(bookingId);
        //         BookingIds = bookingIds.AsReadOnly();
        //         UpdatedAt = DateTime.UtcNow;
        //     }
        // }
        //
        // /// <summary>
        // /// Удаляет идентификатор бронирования у клиента
        // /// </summary>
        // /// <param name="bookingId">Идентификатор бронирования</param>
        // public void RemoveBookingId(Guid bookingId)
        // {
        //     var bookingIds = BookingIds.ToList();
        //     if (bookingIds.Contains(bookingId))
        //     {
        //         bookingIds.Remove(bookingId);
        //         BookingIds = bookingIds.AsReadOnly();
        //         UpdatedAt = DateTime.UtcNow;
        //     }
        // }
        //
        // /// <summary>
        // /// Возвращает полное имя клиента
        // /// </summary>
        // /// <returns>Полное имя клиента (имя и фамилия)</returns>
        // public string GetFullName() => $"{FirstName} {LastName}";
        //


        public override bool Equals(object obj)
        {
            if (obj is ClientEntity other)
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