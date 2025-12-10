using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;

namespace UseCases.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<Client>> CreateClientAsync(string firstName, string lastName, string email, string phoneNumber)
        {
            var firstNameResult = Name.Create(firstName);
            if (firstNameResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации имени: {firstNameResult.Error}");
            }

            var lastNameResult = Name.Create(lastName);
            if (lastNameResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации фамилии: {lastNameResult.Error}");
            }

            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации email: {emailResult.Error}");
            }

            var phoneResult = PhoneNumber.Create(phoneNumber);
            if (phoneResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации номера телефона: {phoneResult.Error}");
            }

            var contactInfoResult = ContactInfo.Create(emailResult.Value, phoneResult.Value);
            if (contactInfoResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка создания контактной информации: {contactInfoResult.Error}");
            }

            var clientResult = Client.Create(firstNameResult.Value, lastNameResult.Value, contactInfoResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Client>(clientResult.Error);
            }

            var saveResult = await _clientRepository.AddAsync(clientResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Client>(saveResult.Error);
            }

            return Result.Success(clientResult.Value);
        }

        public async Task<Result<Client>> UpdateClientAsync(Guid clientId, string firstName, string lastName, string email, string phoneNumber)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации идентификатора клиента: {clientIdResult.Error}");
            }

            var firstNameResult = Name.Create(firstName);
            if (firstNameResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации имени: {firstNameResult.Error}");
            }

            var lastNameResult = Name.Create(lastName);
            if (lastNameResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации фамилии: {lastNameResult.Error}");
            }

            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации email: {emailResult.Error}");
            }

            var phoneResult = PhoneNumber.Create(phoneNumber);
            if (phoneResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации номера телефона: {phoneResult.Error}");
            }

            var contactInfoResult = ContactInfo.Create(emailResult.Value, phoneResult.Value);
            if (contactInfoResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка создания контактной информации: {contactInfoResult.Error}");
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Client>(clientResult.Error);
            }

            var client = clientResult.Value;
            var updateResult = client.UpdateClientData(firstNameResult.Value, lastNameResult.Value, contactInfoResult.Value);
            if (updateResult.IsFailure)
            {
                return Result.Failure<Client>(updateResult.Error);
            }

            var saveResult = await _clientRepository.UpdateAsync(client);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Client>(saveResult.Error);
            }

            return Result.Success(client);
        }

        public async Task<Result> DeleteClientAsync(Guid clientId)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure(clientResult.Error);
            }

            var deleteResult = await _clientRepository.DeleteAsync(clientIdResult.Value);
            return deleteResult;
        }

        public async Task<Result<Client>> GetClientByIdAsync(Guid clientId)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<Client>(clientIdResult.Error);
            }

            return await _clientRepository.GetByIdAsync(clientIdResult.Value);
        }

        public async Task<Result<IEnumerable<Client>>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }
    }
}