using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Client>
    {
        private readonly IClientRepository _clientRepository;

        public CreateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<Client>> HandleAsync(CreateClientCommand command)
        {
            var firstNameResult = Name.Create(command.FirstName);
            if (firstNameResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации имени: {firstNameResult.Error}");
            }

            var lastNameResult = Name.Create(command.LastName);
            if (lastNameResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации фамилии: {lastNameResult.Error}");
            }

            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<Client>($"Ошибка валидации email: {emailResult.Error}");
            }

            var phoneResult = PhoneNumber.Create(command.PhoneNumber);
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
    }
}