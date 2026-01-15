using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Seller;

namespace UseCases.Interfaces.Services;

public interface ISellerService
{
    Task<Result<SellerEntity>> CreateSellerAsync(Guid clientId);
    Task<Result<SellerEntity>> GetSellerByIdAsync(Guid sellerId);
    Task<Result<IEnumerable<SellerEntity>>> GetAllSellersAsync();
    Task<Result> UpdateSellerAsync(Guid sellerId, Guid clientId);
    Task<Result> DeleteSellerAsync(Guid sellerId);
    Task<Result> AddPropertyToSellerAsync(Guid sellerId, Guid propertyId);
    Task<Result> RemovePropertyFromSellerAsync(Guid sellerId, Guid propertyId);
}