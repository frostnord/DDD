using Domain.Domain.Customers.Client;
using Domain.Domain.ValueObjects;
using Presenter.DTOs;

namespace Presenter.Extensions
{
    public static class ClientExtensions
    {
        /// <summary>
        /// Преобразует сущность клиента в DTO
        /// </summary>
        /// <param name="client">Сущность клиента</param>
        /// <returns>ClientDto с данными клиента</returns>
        public static ClientDto ToDTO(this Client client)
        {
            if (client == null)
                return null;

            return new ClientDto
            {
                Id = client.Id.Value,
                FirstName = client.FirstName.Value,
                LastName = client.LastName.Value,
                Email = client.ContactInfo.Email.Value,
                PhoneNumber = client.ContactInfo.PhoneNumber.Value,
                RegisteredDate = client.RegisteredDate,
                UpdatedAt = client.UpdatedAt
            };
        }
    }
}