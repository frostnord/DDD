using System;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий контактную информацию
    /// </summary>
    public class ContactInfo : IEquatable<ContactInfo>
    {
        /// <summary>
        /// Электронная почта
        /// </summary>
        public Email Email { get; }
        
        /// <summary>
        /// Номер телефона
        /// </summary>
        public PhoneNumber PhoneNumber { get; }

        /// <summary>
        /// Создает новый экземпляр контактной информации
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <param name="phoneNumber">Номер телефона</param>
        private ContactInfo(Email email, PhoneNumber phoneNumber)
        {
            Email = email;
            PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра контактной информации с возвратом результата
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <returns>Result с экземпляром ContactInfo при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<ContactInfo> Create(Email email, PhoneNumber phoneNumber)
        {
            var errors = new List<string>();

            if (email == null)
                errors.Add("Email не может быть пустым");

            if (phoneNumber == null)
                errors.Add("Номер телефона не может быть пустым");

            if (errors.Count > 0)
            {
                return Result.Failure<ContactInfo>(string.Join("; ", errors));
            }

            return Result.Success(new ContactInfo(email, phoneNumber));
        }

        public override bool Equals(object obj)
        {
            if (obj is ContactInfo other)
            {
                return Email.Equals(other.Email) &&
                       PhoneNumber.Equals(other.PhoneNumber);
            }
            return false;
        }

        public bool Equals(ContactInfo other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Email.Equals(other.Email) &&
                   PhoneNumber.Equals(other.PhoneNumber);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Email, PhoneNumber);
        }

        public override string ToString() => $"Email: {Email}, Phone: {PhoneNumber}";
    }
}