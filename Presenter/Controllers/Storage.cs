using System.Collections.Concurrent;
using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;
using Presenter.DTOs;
using Presenter.Extensions;
using UseCases.Clients.Commands.CreateClientCommand;
using UseCases.Commands;

namespace Presenter.Controllers;

public class Storage
{
     private static readonly ConcurrentDictionary<ClientId, Client> Clients = new();
     
     static Storage()
     {
      if (Clients.Count > 0) return;
      Result<Client> client = Client.Create(
          Name.Create("John").Value,
          Name.Create("Doe").Value,
          ContactInfo.Create(
                  Email.Create("qa@re.re").Value,
                  PhoneNumber.Create("89233333333").Value)
              .Value);
      
      Clients.TryAdd(client.Value.Id,client.Value);
      Result<Client> client2 = Client.Create(
          Name.Create("John").Value,
          Name.Create("Week").Value,
          ContactInfo.Create(
                  Email.Create("qa@re.re").Value,
                  PhoneNumber.Create("83433344434").Value)
              .Value);
      
      Clients.TryAdd(client2.Value.Id,client2.Value);
      Console.WriteLine("Клиенты созданы");
     }

     public static Result<Client> Add (CreateClientCommand command)
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
         Clients.TryAdd(clientResult.Value.Id,clientResult.Value);
         return Result.Success(clientResult.Value);
     }
     
     public static Result<Client> Update(ClientId clientId, CreateClientCommand command)
     {
         // Проверяем, существует ли клиент
         if (!Clients.TryGetValue(clientId, out var existingClient))
         {
             return Result.Failure<Client>("Клиент не найден");
         }

         // Валидируем новые данные
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

         // Используем метод обновления из доменной модели
         var updateResult = existingClient.UpdateClientData(firstNameResult.Value, lastNameResult.Value, contactInfoResult.Value);
         if (updateResult.IsFailure)
         {
             return Result.Failure<Client>(updateResult.Error);
         }

         return Result.Success(existingClient);
     }

     public static Result<ClientDto> UpdateDto(ClientId clientId, CreateClientCommand command)
     {
         var result = Update(clientId, command);
         if (result.IsFailure)
             return Result.Failure<ClientDto>(result.Error);
         
         // Возвращаем обновленного клиента как DTO
         var updatedClient = result.Value;
         return Result.Success(new ClientDto
         {
             Id = updatedClient.Id.Value,
             FirstName = updatedClient.FirstName.Value,
             LastName = updatedClient.LastName.Value,
             Email = updatedClient.ContactInfo.Email.Value,
             PhoneNumber = updatedClient.ContactInfo.PhoneNumber.Value,
             RegisteredDate = updatedClient.RegisteredDate,
             UpdatedAt = updatedClient.UpdatedAt
         });
     }

     public static Result Remove (ClientId clientId)
     {
         if (Clients.TryRemove(clientId, out _))
         {
             return Result.Success();
         }
         return Result.Failure("Клиент не найден");
     }

     public static Result<Client> Get(ClientId clientId)
     {
         if (Clients.TryGetValue(clientId, out var client))
         {
             return Result.Success(client);
         }
         return Result.Failure<Client>("Клиент не найден");
     }
     public static Result<ICollection<Client>> GetAll()
     {
         return Result.Success(Clients.Values);
     }

     public static Result<ClientDto> GetDto(ClientId clientId)
     {
         var result = Get(clientId);
         if (result.IsFailure)
             return Result.Failure<ClientDto>(result.Error);
         
         return Result.Success(result.Value.ToDTO());
     }

     public static Result<ICollection<ClientDto>> GetAllDtos()
     {
         var allClients = GetAll();
         if (allClients.IsFailure)
             return Result.Failure<ICollection<ClientDto>>(allClients.Error);
         
         var clientDtos = allClients.Value.Select(client => client.ToDTO()).ToList();
         return Result.Success<ICollection<ClientDto>>(clientDtos);
     }


     
}