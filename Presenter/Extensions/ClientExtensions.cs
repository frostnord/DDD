using Domain.Customers.Client;
using Presenter.DTOs;
using Presenter.DTOs.ClientDTO;

namespace Presenter.Extensions
{
    public static class ClientExtensions
    {
        /// <summary>
        /// Преобразует сущность клиента в DTO
        /// </summary>
        /// <param name="clientEntity">Сущность клиента</param>
        /// <returns>ClientDto с данными клиента</returns>
        public static ClientDto? ToDTO(this ClientEntity clientEntity)
        {
            if (clientEntity == null)
                return null;

            return new ClientDto
            (
                clientEntity.Id.Value,
                clientEntity.FirstName.Value,
                clientEntity.LastName.Value,
                clientEntity.ContactInfo.Email.Value,
                clientEntity.ContactInfo.PhoneNumber.Value,
                clientEntity.RegisteredDate,
                clientEntity.UpdatedAt
            );
        }
    }
}