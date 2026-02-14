using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Client.Commands.CreateClient;

public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, ClientEntity>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClientEntity>> HandleAsync(CreateClientCommand command, CancellationToken cancellationToken = default)
    {
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

        var clientResult = ClientEntity.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            contactInfoResult.Value
        );

        if (clientResult.IsFailure)
        {
            return Result.Failure<ClientEntity>(clientResult.Error);
        }

        var saveResult = _unitOfWork.Clients.Add(clientResult.Value);
        if (saveResult.IsFailure)
        {
            return Result.Failure<ClientEntity>(saveResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(clientResult.Value);
    }
}
