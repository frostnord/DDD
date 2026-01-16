﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.SellerDTO
{
    /// <summary>
    /// DTO для запроса обновления продавца
    /// </summary>
    public class UpdateSellerRequest
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        [Required(ErrorMessage = "Идентификатор клиента обязателен")]
        public Guid ClientId { get; init; }
    }
}