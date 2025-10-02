using DDD.Domain.ValueObjects;
using Domain.Entities;

namespace DDD.Domain.Entities
{
    /// <summary>
    /// Сущность агентства недвижимости в системе управления недвижимостью
    /// </summary>
    public class Agency
    {
        /// <summary>
        /// Уникальный идентификатор агентства
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Название агентства
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Контактная информация агентства
        /// </summary>
        public ContactInfo ContactInfo { get; private set; }
        
        /// <summary>
        /// Номер лицензии агентства
        /// </summary>
        public string LicenseNumber { get; private set; }
        
        /// <summary>
        /// Список объектов недвижимости, принадлежащих агентству
        /// </summary>
        public List<RealEstate> Properties { get; private set; }
        
        /// <summary>
        /// Дата создания записи об агентстве
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления записи об агентстве
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр агентства недвижимости
        /// </summary>
        /// <param name="name">Название агентства</param>
        /// <param name="contactInfo">Контактная информация агентства</param>
        /// <param name="licenseNumber">Номер лицензии агентства</param>
        /// <exception cref="ArgumentException">Вызывается, если данные агентства некорректны</exception>
        /// <exception cref="ArgumentNullException">Вызывается, если контактная информация пуста</exception>
        public Agency(string name, ContactInfo contactInfo, string licenseNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название агентства не может быть пустым", nameof(name));
            }
            
            if (contactInfo == null)
            {
                throw new ArgumentNullException(nameof(contactInfo), "Контактная информация не может быть пустой");
            }
            
            if (string.IsNullOrWhiteSpace(licenseNumber))
            {
                throw new ArgumentException("Номер лицензии не может быть пустым", nameof(licenseNumber));
            }
            
            Id = Guid.NewGuid();
            Name = name;
            ContactInfo = contactInfo;
            LicenseNumber = licenseNumber;
            Properties = new List<RealEstate>();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Добавляет объект недвижимости к агентству
        /// </summary>
        /// <param name="property">Объект недвижимости для добавления</param>
        /// <exception cref="ArgumentNullException">Вызывается, если объект недвижимости пуст</exception>
        public void AddProperty(RealEstate property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "Объект недвижимости не может быть пустым");
            }
            
            Properties.Add(property);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Удаляет объект недвижимости из агентства
        /// </summary>
        /// <param name="property">Объект недвижимости для удаления</param>
        /// <exception cref="ArgumentNullException">Вызывается, если объект недвижимости пуст</exception>
        public void RemoveProperty(RealEstate property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "Объект недвижимости не может быть пустым");
            }
            
            Properties.Remove(property);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет контактную информацию агентства
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
    }
}