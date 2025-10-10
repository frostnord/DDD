using CSharpFunctionalExtensions;
using DDD.Domain.Entities;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.AgencyVO;
using Domain.ValueObjects;

namespace DDD.Domain
{
    /// <summary>
    /// Сущность агентства недвижимости в системе управления недвижимостью
    /// </summary>
    public class Agency : Entity<AgencyId>
    {
        
        /// <summary>
        /// Название агентства
        /// </summary>
        public Name Name { get; private set; }
        
        /// <summary>
        /// Контактная информация агентства
        /// </summary>
        public ContactInfo ContactInfo { get; private set; }
        
        /// <summary>
        /// Номер лицензии агентства
        /// </summary>
        public LicenseNumber LicenseNumber { get; private set; }
        
        /// <summary>
        /// Список объектов недвижимости, принадлежащих агентству
        /// </summary>
        public List<Property> Properties { get; private set; }
        
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
        /// <param name="id">Идентификатор агентства</param>
        /// <param name="name">Название агентства</param>
        /// <param name="contactInfo">Контактная информация агентства</param>
        /// <param name="licenseNumber">Номер лицензии агентства</param>
        /// <exception cref="ArgumentException">Вызывается, если данные агентства некорректны</exception>
        /// <exception cref="ArgumentNullException">Вызывается, если контактная информация пуста</exception>
        private Agency(AgencyId id, Name name, ContactInfo contactInfo, LicenseNumber licenseNumber)
            : base(id)
        {
            Name = name;
            ContactInfo = contactInfo;
            LicenseNumber = licenseNumber;
            Properties = new List<Property>();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра агентства с возвратом результата
        /// </summary>
        /// <param name="name">Название агентства</param>
        /// <param name="contactInfo">Контактная информация агентства</param>
        /// <param name="licenseNumber">Номер лицензии агентства</param>
        /// <returns>Result с экземпляром Agency при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Agency> Create(Name name, ContactInfo contactInfo, LicenseNumber licenseNumber)
        {
            var validationErrors = new List<string>();

            if (name == null)
                validationErrors.Add("Название агентства не может быть пустым");

            if (contactInfo == null)
                validationErrors.Add("Контактная информация не может быть пустой");

            if (licenseNumber == null)
                validationErrors.Add("Номер лицензии не может быть пустым");

            var id = AgencyId.New();

            return validationErrors.Count > 0
                ? Result.Failure<Agency>(string.Join("; ", validationErrors))
                : Result.Success(new Agency(id, name, contactInfo, licenseNumber));
        }

        /// <summary>
        /// Добавляет объект недвижимости к агентству
        /// </summary>
        /// <param name="property">Объект недвижимости для добавления</param>
        /// <exception cref="ArgumentNullException">Вызывается, если объект недвижимости пуст</exception>
        public void AddProperty(Property property)
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
        public void RemoveProperty(Property property)
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

        public override bool Equals(object obj)
        {
            if (obj is Agency other)
            {
                return base.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}