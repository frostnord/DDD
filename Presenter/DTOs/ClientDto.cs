using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs
{
    /// <summary>
    /// DTO для представления клиента
    /// </summary>
    public class ClientDto
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email клиента
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegisteredDate { get; set; }
        
        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}