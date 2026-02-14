using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Client.Commands.UpdateClient;

public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand, ClientEntity>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClientEntity>> HandleAsync(UpdateClientCommand command, CancellationToken cancellationToken = default)
    {
        var clientId = ClientId.Create(command.ClientId);
        if (clientId.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid client ID: {clientId.Error}");
        }

        var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientId.Value, cancellationToken);
        if (clientResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Client with ID {command.ClientId} does not exist");
        }

        var firstNameResult = Name.Create(command.FirstName);
        if (firstNameResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid first name: {firstNameResult.Error}");
        }

        var lastNameResult = Name.Create(command.LastName);
        if (lastNameResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid last name: {lastNameResult.Error}");
        }

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid email: {emailResult.Error}");
        }

        var phoneNumberResult = PhoneNumber.Create(command.PhoneNumber);
        if (phoneNumberResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid phone number: {phoneNumberResult.Error}");
        }

        var contactInfoResult = ContactInfo.Create(emailResult.Value, phoneNumberResult.Value);
        if (contactInfoResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid contact info: {contactInfoResult.Error}");
        }

        var updateResult = clientResult.Value.UpdateClientData(
            firstNameResult.Value,
            lastNameResult.Value,
            contactInfoResult.Value
        );

        if (updateResult.IsFailure)
        {
            return Result.Failure<ClientEntity>(updateResult.Error);
        }

        var saveResult = _unitOfWork.Clients.Update(clientResult.Value);
        if (saveResult.IsFailure)
        {
            return Result.Failure<ClientEntity>(saveResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(clientResult.Value);
    }
}
