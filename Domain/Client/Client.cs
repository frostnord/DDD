
using CSharpFunctionalExtensions;
using DDD.Domain.Entities.Deal;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.ClientVO;
using Domain.ValueObjects;

namespace DDD.Domain
{
    /// <summary>
    /// Сущность клиента в системе управления недвижимостью
    /// </summary>
    public class Client : CSharpFunctionalExtensions.Entity<ClientId>
    {
        
        /// <summary>
        /// Имя клиента
        /// </summary>
        public Name FirstName { get; set; }
        
        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public Name LastName { get; set; }
        
        /// <summary>
        /// Контактная информация клиента
        /// </summary>
        public ContactInfo ContactInfo { get; private set; }
        
        /// <summary>
        /// Критерии поиска недвижимости клиентом
        /// </summary>
        public ClientSearchCriteria SearchCriteria { get; private set; }
        
        /// <summary>
        /// Список совершенных сделок клиента
        /// </summary>
        private IReadOnlyList<CompletedDeal> CompletedDeals { get; set; }
        
        /// <summary>
        /// Список идентификаторов бронирований клиента (для связи с агрегатом Booking)
        /// </summary>
        private IReadOnlyList<Guid> BookingIds { get; set; }
        
        /// <summary>
        /// Дата создания записи о клиенте
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления записи о клиенте
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр клиента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstName">Имя клиента</param>
        /// <param name="lastName">Фамилия клиента</param>
        /// <param name="contactInfo">Контактная информация клиента</param>
        /// <param name="searchCriteria">Критерии поиска недвижимости</param>
        private Client(ClientId id, Name firstName, Name lastName, ContactInfo contactInfo, ClientSearchCriteria searchCriteria = null)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
            SearchCriteria = searchCriteria;
            CreatedAt = DateTime.UtcNow;
            CompletedDeals = new List<CompletedDeal>().AsReadOnly();
            BookingIds = new List<Guid>().AsReadOnly();
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра клиента с возвратом результата
        /// </summary>
        /// <param name="firstName">Имя клиента</param>
        /// <param name="lastName">Фамилия клиента</param>
        /// <param name="contactInfo">Контактная информация клиента</param>
        /// <param name="searchCriteria">Критерии поиска недвижимости</param>
        /// <returns>Result с экземпляром Client при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Client> Create(Name firstName, Name lastName, ContactInfo contactInfo, ClientSearchCriteria searchCriteria = null)
        {
            var validationErrors = new List<string>();

            // Валидация входных параметров
            if (firstName == null)
                validationErrors.Add("Имя клиента не может быть пустым");

            if (lastName == null)
                validationErrors.Add("Фамилия клиента не может быть пустой");

            if (contactInfo == null)
                validationErrors.Add("Контактная информация не может быть пустой");

            var id = ClientId.New();

            // Возврат результата валидации
            return validationErrors.Count > 0
                ? Result.Failure<Client>(string.Join("; ", validationErrors))
                : Result.Success(new Client(id, firstName, lastName, contactInfo, searchCriteria));
        }

        /// <summary>
        /// Обновляет контактную информацию клиента
        /// </summary>
        /// <param name="newContactInfo">Новая контактная информация</param>
        /// <exception cref="ArgumentNullException">Вызывается, если новая контактная информация пуста</exception>
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
        /// Обновляет критерии поиска клиента
        /// </summary>
        /// <param name="newSearchCriteria">Новые критерии поиска</param>
        public void UpdateSearchCriteria(ClientSearchCriteria newSearchCriteria)
        {
            SearchCriteria = newSearchCriteria;
            UpdatedAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Добавляет совершенную сделку клиенту
        /// </summary>
        /// <param name="deal">Совершенная сделка</param>
        public void AddCompletedDeal(CompletedDeal deal)
        {
            if (deal == null)
                throw new ArgumentNullException(nameof(deal), "Сделка не может быть пустой");
                
            var deals = CompletedDeals.ToList();
            if (!deals.Contains(deal))
            {
                deals.Add(deal);
                CompletedDeals = deals.AsReadOnly();
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// Удаляет совершенную сделку у клиента
        /// </summary>
        /// <param name="dealId">Идентификатор сделки</param>
        public void RemoveCompletedDeal(CompletedDealId dealId)
        {
            var deals = CompletedDeals.ToList();
            var dealToRemove = deals.FirstOrDefault(d => d.Id == dealId);
            if (dealToRemove != null)
            {
                deals.Remove(dealToRemove);
                CompletedDeals = deals.AsReadOnly();
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// Добавляет идентификатор бронирования к клиенту
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования</param>
        public void AddBookingId(Guid bookingId)
        {
            var bookingIds = BookingIds.ToList();
            if (!bookingIds.Contains(bookingId))
            {
                bookingIds.Add(bookingId);
                BookingIds = bookingIds.AsReadOnly();
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// Удаляет идентификатор бронирования у клиента
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования</param>
        public void RemoveBookingId(Guid bookingId)
        {
            var bookingIds = BookingIds.ToList();
            if (bookingIds.Contains(bookingId))
            {
                bookingIds.Remove(bookingId);
                BookingIds = bookingIds.AsReadOnly();
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        /// <summary>
        /// Возвращает полное имя клиента
        /// </summary>
        /// <returns>Полное имя клиента (имя и фамилия)</returns>
        public string GetFullName() => $"{FirstName} {LastName}";

        // public override bool Equals(object obj)
        // {
        //     if (obj is Client other)
        //     {
        //         return Id.Equals(other.Id);
        //     }
        //     return false;
        // }
        //
        // public override int GetHashCode()
        // {
        //     return Id.GetHashCode();
        // }
    }
}