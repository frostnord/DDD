using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;
using Domain.Property.VO;
using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace UseCases.Services
{
    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IClientRepository _clientRepository;

        public SellerService(ISellerRepository sellerRepository, IClientRepository clientRepository)
        {
            _sellerRepository = sellerRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<SellerEntity>> CreateSellerAsync(Guid clientId)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<SellerEntity>($"Client with ID {clientId} does not exist");
            }

            var sellerResult = SellerEntity.Create(clientIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(sellerResult.Error);
            }

            var saveResult = await _sellerRepository.AddAsync(sellerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(saveResult.Error);
            }

            return Result.Success(sellerResult.Value);
        }

        public async Task<Result<SellerEntity>> GetSellerByIdAsync(Guid sellerId)
        {
            var sellerIdResult = SellerId.Create(sellerId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(sellerIdResult.Error);
            }

            return await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
        }

        public async Task<Result<IEnumerable<SellerEntity>>> GetAllSellersAsync()
        {
            return await _sellerRepository.GetAllAsync();
        }

        public async Task<Result> UpdateSellerAsync(Guid sellerId, Guid clientId)
        {
            var sellerIdResult = SellerId.Create(sellerId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure(sellerIdResult.Error);
            }

            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure($"Client with ID {clientId} does not exist");
            }

            var sellerResult = await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure(sellerResult.Error);
            }

            // В текущей модели Seller не имеет метода обновления, поэтому просто возвращаем успех
            // Если потребуется обновление, нужно будет добавить метод в сущность Seller
            return Result.Success();
        }

        public async Task<Result> DeleteSellerAsync(Guid sellerId)
        {
            var sellerIdResult = SellerId.Create(sellerId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure(sellerIdResult.Error);
            }

            var sellerResult = await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure(sellerResult.Error);
            }

            var deleteResult = await _sellerRepository.DeleteAsync(sellerIdResult.Value);
            return deleteResult;
        }

        public async Task<Result> AddPropertyToSellerAsync(Guid sellerId, Guid propertyId)
        {
            var sellerIdResult = SellerId.Create(sellerId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure(sellerIdResult.Error);
            }

            var propertyIdResult = PropertyId.Create(propertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure(propertyIdResult.Error);
            }

            var sellerResult = await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure(sellerResult.Error);
            }

            var attachResult = sellerResult.Value.AttachProperty(propertyIdResult.Value);
            if (attachResult.IsFailure)
            {
                return Result.Failure(attachResult.Error);
            }

            // Обновляем продавца в репозитории
            var updateResult = await _sellerRepository.UpdateAsync(sellerResult.Value);
            return updateResult;
        }

        public async Task<Result> RemovePropertyFromSellerAsync(Guid sellerId, Guid propertyId)
        {
            var sellerIdResult = SellerId.Create(sellerId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure(sellerIdResult.Error);
            }

            var propertyIdResult = PropertyId.Create(propertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure(propertyIdResult.Error);
            }

            var sellerResult = await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure(sellerResult.Error);
            }

            var detachResult = sellerResult.Value.DetachProperty(propertyIdResult.Value);
            if (detachResult.IsFailure)
            {
                return Result.Failure(detachResult.Error);
            }

            // Обновляем продавца в репозитории
            var updateResult = await _sellerRepository.UpdateAsync(sellerResult.Value);
            return updateResult;
        }
    }
}