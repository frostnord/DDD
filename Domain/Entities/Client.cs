using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// Сущность клиента в системе управления недвижимостью
    /// </summary>
    public class Client
    {
        private readonly List<Guid> _bookingIds;

        /// <summary>
        /// Уникальный идентификатор клиента
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Имя клиента
        /// </summary>
        public Name FirstName { get; private set; }
        
        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public Name LastName { get; private set; }
        
        /// <summary>
        /// Контактная информация клиента
        /// </summary>
        public ContactInfo ContactInfo { get; private set; }
        
        /// <summary>
        /// Критерии поиска недвижимости клиентом
        /// </summary>
        public ClientSearchCriteria SearchCriteria { get; private set; }
        
        /// <summary>
        /// Список идентификаторов бронирований клиента (для связи с агрегатом Booking)
        /// </summary>
        public IReadOnlyList<Guid> BookingIds => _bookingIds.AsReadOnly();
        
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
        /// <param name="firstName">Имя клиента</param>
        /// <param name="lastName">Фамилия клиента</param>
        /// <param name="contactInfo">Контактная информация клиента</param>
        /// <param name="searchCriteria">Критерии поиска недвижимости</param>
        private Client(Name firstName, Name lastName, ContactInfo contactInfo, ClientSearchCriteria searchCriteria = null)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
            SearchCriteria = searchCriteria;
            CreatedAt = DateTime.UtcNow;
            _bookingIds = new List<Guid>();
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

            // Возврат результата валидации
            return validationErrors.Count > 0
                ? Result.Failure<Client>(string.Join("; ", validationErrors))
                : Result.Success(new Client(firstName, lastName, contactInfo, searchCriteria));
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
        /// Добавляет идентификатор бронирования к клиенту
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования</param>
        public void AddBookingId(Guid bookingId)
        {
            if (!_bookingIds.Contains(bookingId))
            {
                _bookingIds.Add(bookingId);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Удаляет идентификатор бронирования у клиента
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования</param>
        public void RemoveBookingId(Guid bookingId)
        {
            if (_bookingIds.Contains(bookingId))
            {
                _bookingIds.Remove(bookingId);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Возвращает полное имя клиента
        /// </summary>
        /// <returns>Полное имя клиента (имя и фамилия)</returns>
        public string GetFullName() => $"{FirstName} {LastName}";

        public override bool Equals(object obj)
        {
            if (obj is Client other)
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