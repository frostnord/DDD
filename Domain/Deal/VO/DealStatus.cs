using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;
using DDD.Utilities;

namespace Domain.ValueObjects
{
    
    public abstract class DealStatus : Enumeration<DealStatus>
    {
        protected DealStatus(int key, string name):base( key, name){}
        
        public abstract bool IsActive();
    }
    
    public sealed class DealStatusCreated : DealStatus
    {
        public DealStatusCreated() : base(0, "Создана") { }

        public override bool IsActive() => true;
    }
    
    public sealed class DealStatusConfirmed : DealStatus
    {
        public DealStatusConfirmed() : base(1, "Подтверждена") { }
        public override bool IsActive() => true;
    }
    
    public sealed class DealStatusCompleted:DealStatus
    {
        public DealStatusCompleted() : base(2, "Завершена") { }
        public override bool IsActive() => false;
    }
    
    public sealed class DealStatusCancelled:DealStatus
    {
        public DealStatusCancelled() : base(3, "Отменена") { }
        public override bool IsActive() => false;
    }
    
    
    // /// <summary>
    // /// Объект значения, представляющий статус сделки
    // /// </summary>
    // public class DealStatus : ValueObject
    // {
    //     /// <summary>
    //     /// Статус: Создана
    //     /// </summary>
    //     public static readonly DealStatus Created = new DealStatus("Created", "Создана");
    //     
    //     /// <summary>
    //     /// Статус: Подтверждена
    //     /// </summary>
    //     public static readonly DealStatus Confirmed = new DealStatus("Confirmed", "Подтверждена");
    //     
    //     /// <summary>
    //     /// Статус: Завершена
    //     /// </summary>
    //     public static readonly DealStatus Completed = new DealStatus("Completed", "Завершена");
    //     
    //     /// <summary>
    //     /// Статус: Отменена
    //     /// </summary>
    //     public static readonly DealStatus Cancelled = new DealStatus("Cancelled", "Отменена");
    //
    //     /// <summary>
    //     /// Код статуса
    //     /// </summary>
    //     public string Code { get; }
    //     
    //     /// <summary>
    //     /// Отображаемое имя статуса
    //     /// </summary>
    //     public string DisplayName { get; }
    //
    //     /// <summary>
    //     /// Создает новый экземпляр статуса сделки
    //     /// </summary>
    //     /// <param name="code">Код статуса</param>
    //     /// <param name="displayName">Отображаемое имя</param>
    //     private DealStatus(string code, string displayName)
    //     {
    //         Code = code;
    //         DisplayName = displayName;
    //     }
    //
    //     /// <summary>
    //     /// Получает статус сделки по коду
    //     /// </summary>
    //     /// <param name="code">Код статуса</param>
    //     /// <returns>Статус сделки или null, если не найден</returns>
    //     public static DealStatus FromCode(string code)
    //     {
    //         switch (code?.ToLower())
    //         {
    //             case "created":
    //                 return Created;
    //             case "confirmed":
    //                 return Confirmed;
    //             case "completed":
    //                 return Completed;
    //             case "cancelled":
    //                 return Cancelled;
    //             default:
    //                 return null;
    //         }
    //     }
    //
    //     protected override IEnumerable<object> GetEqualityComponents()
    //     {
    //         yield return Code;
    //     }
    //
    //     public override string ToString()
    //     {
    //         return DisplayName;
    //     }
    // }
}