using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий адрес недвижимости
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; }
        
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; }
        
        /// <summary>
        /// Штат/область
        /// </summary>
        public int HomeNumber { get; }
        
        /// <summary>
        /// Почтовый индекс
        /// </summary>
        public int ZipCode { get; }
        
        /// <summary>
        /// Страна
        /// </summary>
        public string Country { get; }

        /// <summary>
        /// Создает новый экземпляр адреса
        /// </summary>
        /// <param name="street">Улица</param>
        /// <param name="city">Город</param>
        /// <param name="homeNumber">Штат/область</param>
        /// <param name="zipCode">Почтовый индекс</param>
        /// <param name="country">Страна</param>
        private Address(string street, string city, int homeNumber, int zipCode, string country)
        {
            
            Street = street;
            City = city;
            HomeNumber = homeNumber;
            ZipCode = zipCode;
            Country = country;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра адреса с возвратом результата
        /// </summary>
        /// <param name="street">Улица</param>
        /// <param name="city">Город</param>
        
        /// <param name="homeNumber"></param>
        /// <param name="zipCode">Почтовый индекс</param>
        /// <param name="country">Страна</param>
        /// <returns>Result с экземпляром Address при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Address> Create(string street, string city, int homeNumber, int zipCode, string country)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(street))
                errors.Add("Улица не может быть пустой");
            if (string.IsNullOrWhiteSpace(city))
                errors.Add("Город не может быть пустым");
            if (homeNumber == null)
                errors.Add("Номер дома не может быть пустой");
            if (zipCode== null)
                errors.Add("Почтовый индекс не может быть пустым");
            if (string.IsNullOrWhiteSpace(country))
                errors.Add("Страна не может быть пустой");

            return errors.Count > 0
               ? Result.Failure<Address>(string.Join("; ", errors))
               : Result.Success(new Address(street, city, homeNumber, zipCode, country));
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {HomeNumber}, {ZipCode}, {Country}";
        }
    }
}