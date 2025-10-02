using System;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// Сущность клиента в системе управления недвижимостью
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Уникальный идентификатор клиента
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; private set; }
        
        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string LastName { get; private set; }
        
        /// <summary>
        /// Контактная информация клиента
        /// </summary>
        public ContactInfo ContactInfo { get; private set; }
        
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
        /// <exception cref="ArgumentException">Вызывается, если данные клиента некорректны</exception>
        /// <exception cref="ArgumentNullException">Вызывается, если контактная информация пуста</exception>
        public Client(string firstName, string lastName, ContactInfo contactInfo)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Имя клиента не может быть пустым", nameof(firstName));
            }
            
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Фамилия клиента не может быть пустой", nameof(lastName));
            }
            
            if (contactInfo == null)
            {
                throw new ArgumentNullException(nameof(contactInfo), "Контактная информация не может быть пустой");
            }
            
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
            CreatedAt = DateTime.UtcNow;
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
        /// Возвращает полное имя клиента
        /// </summary>
        /// <returns>Полное имя клиента (имя и фамилия)</returns>
        public string GetFullName() => $"{FirstName} {LastName}";
    }
}