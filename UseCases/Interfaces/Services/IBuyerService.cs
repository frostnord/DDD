using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;

namespace UseCases.Interfaces.Services
{
    public interface IBuyerService
    {
        Task<Result<BuyerEntity>> CreateBuyerAsync(Guid clientId, int preferredNumberOfRooms, int preferredFloor,
            int preferredTotalFloors, string preferredType, string preferredHeatingType, string preferredCondition,
            bool? preferParking);

        Task<Result<BuyerEntity>> GetBuyerByIdAsync(Guid buyerId);
        Task<Result<IEnumerable<BuyerEntity>>> GetAllBuyersAsync();

        Task<Result> UpdateBuyerAsync(Guid buyerId, Guid clientId, int preferredNumberOfRooms, int preferredFloor,
            int preferredTotalFloors, string preferredType, string preferredHeatingType, string preferredCondition,
            bool? preferParking);

        Task<Result> DeleteBuyerAsync(Guid buyerId);
    }
}