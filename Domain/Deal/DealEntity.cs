using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal.VO;
using Domain.Property.VO;

namespace Domain.Deal
{
    /// <summary>
    /// Сущность сделки в системе управления недвижимостью
    /// Объединяет Property, Client и документы в единую сделку
    /// </summary>
    public class DealEntity : Entity<DealId>
    {
        /// <summary>
        /// Идентификатор клиента, участвующего в сделке
        /// </summary>
        public ClientId ClientId { get; private set; }

        /// <summary>
        /// Идентификатор объекта недвижимости, участвующего в сделке
        /// </summary>
        public PropertyId PropertyId { get; private set; }

        /// <summary>
        /// Детали сделки
        /// </summary>
        public DealDetails Details { get; private set; }

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
        /// Создает новый экземпляр сделки
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="details">Детали сделки</param>
        protected DealEntity(DealId id, ClientId clientId, PropertyId propertyId, DealDetails details)
            : base(id)
        {
            ClientId = clientId;
            PropertyId = propertyId;
            Details = details;
            Status = DealStatus.Created;
            CreatedAt = DateTime.UtcNow;
        }

        // EF Core конструктор
        protected DealEntity()
        {
        }

        /// <summary>
        /// Создает новый экземпляр сделки через фабричный метод
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <param name="details">Детали сделки</param>
        /// <returns>Результат с сделкой или ошибкой</returns>
        public static Result<DealEntity> Create(ClientId clientId, PropertyId propertyId, DealDetails details)
        {
            var validationErrors = new List<string>();

            if (clientId == null || clientId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор клиента не может быть пустым");

            if (propertyId == null || propertyId.Value == Guid.Empty)
                validationErrors.Add("Идентификатор объекта недвижимости не может быть пустым");

            if (details == null)
                validationErrors.Add("Детали сделки не могут быть пустыми");

            var id = DealId.Create(Guid.NewGuid()).Value;

            if (validationErrors.Count > 0)
            {
                return Result.Failure<DealEntity>(string.Join("; ", validationErrors));
            }

            var deal = new DealEntity(id, clientId, propertyId, details);
            return Result.Success(deal);
        }

        // public void Close()
        // {
        //     if (Status != DealStatus.Completed)
        //         throw new InvalidDealStateException("Закрыть можно только активную сделку.");
        //
        //     Status = DealStatus.Completed;
        //     ClosedAt = DateTime.UtcNow;
        //
        //     AddEvent(new DealClosedEvent(Id.Value, ClosedAt.Value));
        // }
        //
        // public void Cancel(string reason)
        // {
        //     if (Status != DealStatus.Created)
        //         throw new InvalidDealStateException("Отменить можно только активную сделку.");
        //
        //     Status = DealStatus.Cancelled;
        //
        //     AddEvent(new DealCancelledEvent(Id.Value, reason));
        // }
        //
        // private void AddEvent(IDomainEvent @event) => _events.Add(@event);
        // public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

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
            if (obj is DealEntity other)
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