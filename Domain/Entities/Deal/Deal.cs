using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DDD.Domain.Entities;
using DDD.Domain.Aggregates;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// Сущность сделки в системе управления недвижимостью
    /// Объединяет Property, Client, Booking и документы в единую сделку
    /// </summary>
    public class Deal
    {
        private List<Document> _documents;

        /// <summary>
        /// Уникальный идентификатор сделки
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Идентификатор клиента, участвующего в сделке
        /// </summary>
        public Guid ClientId { get; private set; }
        
        /// <summary>
        /// Идентификатор объекта недвижимости, участвующего в сделке
        /// </summary>
        public Guid PropertyId { get; private set; }
        
        /// <summary>
        /// Идентификатор бронирования, связанного со сделкой
        /// </summary>
        public Guid? BookingId { get; private set; }
        
        /// <summary>
        /// Детали сделки
        /// </summary>
        public DealDetails Details { get; private set; }
        
        /// <summary>
        /// Список документов, связанных со сделкой
        /// </summary>
        public IReadOnlyList<Document> Documents => _documents.AsReadOnly();
        
        /// <summary>
        /// Статус сделки
        /// </summary>
        public DealStatus Status { get; private set; }
        
        /// <summary>
        /// Дата создания сделки
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления сделки
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр сделки через фабричный метод
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="bookingId">Идентификатор бронирования (опционально)</param>
        /// <param name="details">Детали сделки</param>
        /// <returns>Результат с сделкой или ошибкой</returns>
        public static Result<Deal> Create(Guid clientId, Guid propertyId, Guid? bookingId, DealDetails details)
        {
            var validationErrors = new List<string>();

            if (clientId == Guid.Empty)
                validationErrors.Add("Идентификатор клиента не может быть пустым");

            if (propertyId == Guid.Empty)
                validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

            if (details == null)
                validationErrors.Add("Детали сделки не могут быть пустыми");

            if (validationErrors.Count > 0)
            {
                return Result.Failure<Deal>(string.Join("; ", validationErrors));
            }

            var deal = new Deal(clientId, propertyId, bookingId, details);
            return Result.Success(deal);
        }

        /// <summary>
        /// Создает новый экземпляр сделки
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="bookingId">Идентификатор бронирования (опционально)</param>
        /// <param name="details">Детали сделки</param>
        private Deal(Guid clientId, Guid propertyId, Guid? bookingId, DealDetails details)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
            PropertyId = propertyId;
            BookingId = bookingId;
            Details = details;
            Status = DealStatus.Created;
            CreatedAt = DateTime.UtcNow;
            _documents = new List<Document>();
        }

        /// <summary>
        /// Добавляет документ к сделке
        /// </summary>
        /// <param name="document">Документ для добавления</param>
        public void AddDocument(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document), "Документ не может быть пустым");
                
            if (!_documents.Contains(document))
            {
                _documents.Add(document);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Удаляет документ из сделки
        /// </summary>
        /// <param name="documentId">Идентификатор документа для удаления</param>
        public void RemoveDocument(Guid documentId)
        {
            var documentToRemove = _documents.FirstOrDefault(d => d.Id == documentId);
            if (documentToRemove != null)
            {
                _documents.Remove(documentToRemove);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Подтверждает сделку
        /// </summary>
        public void Confirm()
        {
            Status = DealStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Завершает сделку
        /// </summary>
        public void Complete()
        {
            Status = DealStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Отменяет сделку
        /// </summary>
        public void Cancel()
        {
            Status = DealStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            if (obj is Deal other)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}