using System.Diagnostics;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;

namespace DDD.Domain.Entities
{
    /// <summary>
    /// Сущность объекта недвижимости в системе управления недвижимостью
    /// </summary>
    public class RealEstate
    {
        /// <summary>
        /// Уникальный идентификатор объекта недвижимости
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Адрес объекта недвижимости
        /// </summary>
        public Address Address { get; private set; }
        
        /// <summary>
        /// Цена объекта недвижимости
        /// </summary>
        public Price Price { get; private set; }
        
        /// <summary>
        /// Статус объекта недвижимости
        /// </summary>
        public PropertyStatus Status { get; private set; }
        
        /// <summary>
        /// Описание объекта недвижимости
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// Тип недвижимости (квартира, дом, коммерческое помещение)
        /// </summary>
        public string Type { get; private set; }
        
        /// <summary>
        /// Площадь в квадратных метрах
        /// </summary>
        public int Area { get; private set; }
        
        /// <summary>
        /// Доступность объекта недвижимости
        /// </summary>
        public bool IsAvailable { get; private set; }
        
        /// <summary>
        /// Дата создания записи об объекте недвижимости
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления записи об объекте недвижимости
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр объекта недвижимости
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="type">Тип недвижимости</param>
        /// <param name="area">Площадь в квадратных метрах</param>
        private RealEstate(Address address, Price price, string description, string type, int area)
        {
            Id = Guid.NewGuid();
            Address = address;
            Price = price; //
            Description = description; // 
            Type = type; // Тип 
            Area = area; //площадь помещения 
            IsAvailable = true; 
            CreatedAt = DateTime.UtcNow;
            Status = PropertyStatus.ForSale;
        }
        
        /// <summary>
        /// Фабричный метод для создания экземпляра объекта недвижимости с возвратом результата
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="type">Тип недвижимости</param>
        /// <param name="area">Площадь в квадратных метрах</param>
        /// <returns>Result с экземпляром RealEstate при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<RealEstate> Create(Address address, Price price, string description, string type, int area)
        {
            //Проверки на нул не нужны так-как обж-вал. Обеспечивает его наличие
            var errors = new List<string>();

            if (price == null)
                errors.Add("Цена не может быть пустой");

            // Проверка строковых значений на пустоту
            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Описание не может быть пустым");

            if (string.IsNullOrWhiteSpace(type))
                errors.Add("Тип недвижимости не может быть пустым");

            // Проверка числовых значений на корректность
            if (area <= 0)
                errors.Add("Площадь должна быть положительной");

            // Возврат результата валидации
            return errors.Count > 0
                ? Result.Failure<RealEstate>(string.Join("; ", errors))
                : Result.Success(new RealEstate(address, price, description, type, area ));
        }

        /// <summary>
        /// Обновляет цену объекта недвижимости
        /// </summary>
        /// <param name="newPrice">Новая цена</param>
        /// <exception cref="ArgumentNullException">Вызывается, если новая цена пуста</exception>
        public void UpdatePrice(Price newPrice)
        {
            if (newPrice == null)
            {
                throw new ArgumentNullException(nameof(newPrice), "Цена не может быть пустой");
            }
            
            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет доступность объекта недвижимости
        /// </summary>
        /// <param name="isAvailable">Доступность объекта</param>
        public void UpdateAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет описание объекта недвижимости
        /// </summary>
        /// <param name="newDescription">Новое описание</param>
        /// <exception cref="ArgumentException">Вызывается, если новое описание пусто</exception>
        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                throw new ArgumentException("Описание не может быть пустым", nameof(newDescription));
            }
            
            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }
        public override string ToString()
        {
            return $"Недвижимость [ID: {Id}, Адрес: {Address}, Цена: {Price}, Статус: {Status}, Тип: {Type}, Площадь: {Area} м², Доступна: {IsAvailable}]";
        }
    }

    
}