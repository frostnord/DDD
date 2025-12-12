using CSharpFunctionalExtensions;
using Domain.Agency.VO;
using Domain.Customers.Client.VO;
using Domain.ValueObjects;

namespace Domain.Agency
{
    /// <summary>
    /// Сущность агентства недвижимости в системе управления недвижимостью
    /// </summary>
    /// 
    public class AgencyEntity : Entity<AgencyId>
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
        public List<Property.PropertyEntity> Properties { get; private set; }

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
        private AgencyEntity(AgencyId id, Name name, ContactInfo contactInfo, LicenseNumber licenseNumber)
            : base(id)
        {
            Name = name;
            ContactInfo = contactInfo;
            LicenseNumber = licenseNumber;
            Properties = new List<Property.PropertyEntity>();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра агентства с возвратом результата
        /// </summary>
        /// <param name="name">Название агентства</param>
        /// <param name="contactInfo">Контактная информация агентства</param>
        /// <param name="licenseNumber">Номер лицензии агентства</param>
        /// <returns>Result с экземпляром Agency при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<AgencyEntity> Create(Name name, ContactInfo contactInfo, LicenseNumber licenseNumber)
        {
            var validationErrors = new List<string>();

            if (name == null)
                validationErrors.Add("Название агентства не может быть пустым");

            if (contactInfo == null)
                validationErrors.Add("Контактная информация не может быть пустой");

            if (licenseNumber == null)
                validationErrors.Add("Номер лицензии не может быть пустым");

            var id = AgencyId.Create(Guid.NewGuid()).Value;

            return validationErrors.Count > 0
                ? Result.Failure<AgencyEntity>(string.Join("; ", validationErrors))
                : Result.Success(new AgencyEntity(id, name, contactInfo, licenseNumber));
        }

        /// <summary>
        /// Добавляет объект недвижимости к агентству
        /// </summary>
        /// <param name="propertyEntity">Объект недвижимости для добавления</param>
        /// <exception cref="ArgumentNullException">Вызывается, если объект недвижимости пуст</exception>
        public void AddProperty(Property.PropertyEntity propertyEntity)
        {
            if (propertyEntity == null)
            {
                throw new ArgumentNullException(nameof(propertyEntity), "Объект недвижимости не может быть пустым");
            }

            Properties.Add(propertyEntity);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Удаляет объект недвижимости из агентства
        /// </summary>
        /// <param name="propertyEntity">Объект недвижимости для удаления</param>
        /// <exception cref="ArgumentNullException">Вызывается, если объект недвижимости пуст</exception>
        public void RemoveProperty(Property.PropertyEntity propertyEntity)
        {
            if (propertyEntity == null)
            {
                throw new ArgumentNullException(nameof(propertyEntity), "Объект недвижимости не может быть пустым");
            }

            Properties.Remove(propertyEntity);
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