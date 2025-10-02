using System.Net.Mail;
using CSharpFunctionalExtensions;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий контактную информацию
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; }
        
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; }

        /// <summary>
        /// Создает новый экземпляр контактной информации
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <exception cref="ArgumentException">Вызывается, если контактная информация некорректна</exception>
        public ContactInfo(string email, string phoneNumber)
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
        public static Result<ContactInfo> Create(string email, string phoneNumber)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email не может быть пустым");
            else if (!IsValidEmail(email))
                errors.Add("Некорректный формат email");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                errors.Add("Номер телефона не может быть пустым");

            if (errors.Count > 0)
            {
                return Result.Failure<ContactInfo>(string.Join("; ", errors));
            }

            return Result.Success(new ContactInfo(email, phoneNumber));
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}