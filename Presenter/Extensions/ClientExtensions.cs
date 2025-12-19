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
        public static ClientDto ToDTO(this ClientEntity clientEntity)
        {
            if (clientEntity == null)
                return null;

            return new ClientDto
            {
                Id = clientEntity.Id.Value,
                FirstName = clientEntity.FirstName.Value,
                LastName = clientEntity.LastName.Value,
                Email = clientEntity.ContactInfo.Email.Value,
                PhoneNumber = clientEntity.ContactInfo.PhoneNumber.Value,
                RegisteredDate = clientEntity.RegisteredDate,
                UpdatedAt = clientEntity.UpdatedAt
            };
        }
    }
}