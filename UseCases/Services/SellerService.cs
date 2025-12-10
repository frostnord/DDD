using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Customers.Seller;
using Domain.Domain.Customers.Seller.VO;
using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;

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

        public async Task<Result<Seller>> CreateSellerAsync(Guid clientId)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<Seller>(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Seller>($"Client with ID {clientId} does not exist");
            }

            var sellerResult = Seller.Create(clientIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure<Seller>(sellerResult.Error);
            }

            var saveResult = await _sellerRepository.AddAsync(sellerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Seller>(saveResult.Error);
            }

            return Result.Success(sellerResult.Value);
        }

        public async Task<Result<Seller>> GetSellerByIdAsync(Guid sellerId)
        {
            var sellerIdResult = SellerId.Create(sellerId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure<Seller>(sellerIdResult.Error);
            }

            return await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
        }

        public async Task<Result<IEnumerable<Seller>>> GetAllSellersAsync()
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
    }
}