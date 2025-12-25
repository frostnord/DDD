using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.ClientDTO
{
    /// <summary>
    /// DTO для представления клиента
    /// </summary>
    public record ClientDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        DateTime RegisteredDate,
        DateTime? UpdatedAt);
}