using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для информации о владельце.
    /// </summary>
    public record OwnershipDto
    {
        /// <summary>
        /// Уникальный идентификатор клиента, который является владельцем.
        /// </summary>
        /// <example>a1b2c3d4-e5f6-7890-1234-567890abcdef</example>
        [Required(ErrorMessage = "ID владельца обязателен")]
        public Guid OwnerClientId { get; init; }

        /// <summary>
        /// Дата, с которой начинается владение.
        /// </summary>
        /// <example>2023-01-15T10:00:00Z</example>
        [Required(ErrorMessage = "Дата начала владения обязательна")]
        public DateTime StartDate { get; init; }
    }
}