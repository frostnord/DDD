using DDD.Domain.Entities;
using DDD.Domain.ValueObjects;

namespace DDD;

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

        var realEstateResult = RealEstate.Create(addressResult.Value, priceResult.Value, "description", "type", 100);
        if (realEstateResult.IsFailure)
        {
            Console.WriteLine($"Ошибка при создании недвижимости: {realEstateResult.Error}");
            return;
        }

        var realEstate = realEstateResult.Value;
        Console.WriteLine(realEstate.ToString());
    }
}