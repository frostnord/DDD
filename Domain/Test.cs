using DDD.Domain.Aggregates;
using DDD.Domain.Entities;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;

namespace DDD.Domain;

public class Test
{
    public static void Main()
    {
        var addressResult = Address.Create("street", "city", "state", "zip", "country");
        if (addressResult.IsFailure)
        {
            Console.WriteLine($"Ошибка при создании адреса: {addressResult.Error}");
            return;
        }

        var priceResult = Price.Create(100);
        if (priceResult.IsFailure)
        {
            Console.WriteLine($"Ошибка при создании цены: {priceResult.Error}");
            return;
        }

        var realEstateResult = Property.Create(addressResult.Value, priceResult.Value, "description", "type", 100, "Иванов Иван Иванович", DateTime.Now, "Покупка");
        if (realEstateResult.IsFailure)
        {
            Console.WriteLine($"Ошибка при создании недвижимости: {realEstateResult.Error}");
            return;
        }

        var realEstate = realEstateResult.Value;
        Console.WriteLine(realEstate.ToString());

        Console.WriteLine($"Текущий владелец: {realEstate.GetCurrentOwner().OwnerName}");
    }
}