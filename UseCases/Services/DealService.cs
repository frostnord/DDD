using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;

namespace UseCases.Services
{
    public class DealService : IDealService
    {
        private readonly IDealRepository _dealRepository;

        public DealService(IDealRepository dealRepository)
        {
            _dealRepository = dealRepository;
        }

        public async Task<Result<DealEntity>> GetByIdAsync(Guid dealId)
        {
            var idResult = DealId.Create(dealId);
            if (idResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Invalid deal ID: {dealId}");
            }

            return await _dealRepository.GetByIdAsync(idResult.Value);
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(Guid clientId)
        {
            var idResult = ClientId.Create(clientId);
            if (idResult.IsFailure)
            {
                return Result.Failure<IEnumerable<DealEntity>>($"Invalid client ID: {clientId}");
            }

            return await _dealRepository.GetByClientIdAsync(idResult.Value);
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(Guid propertyId)
        {
            var idResult = PropertyId.Create(propertyId);
            if (idResult.IsFailure)
            {
                return Result.Failure<IEnumerable<DealEntity>>($"Invalid property ID: {propertyId}");
            }

            return await _dealRepository.GetByPropertyIdAsync(idResult.Value);
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetAllAsync()
        {
            return await _dealRepository.GetAllAsync();
        }

        public async Task<Result<DealEntity>> CreateAsync(Guid clientId, Guid propertyId, Guid? bookingId, DealDetails details)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Invalid client ID: {clientId}");
            }

            var propertyIdResult = PropertyId.Create(propertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Invalid property ID: {propertyId}");
            }

            BookingId? bookingIdObj = null;
            if (bookingId.HasValue)
            {
                var bookingIdResult = BookingId.Create(bookingId.Value);
                if (bookingIdResult.IsFailure)
                {
                    return Result.Failure<DealEntity>($"Invalid booking ID: {bookingId.Value}");
                }
                bookingIdObj = bookingIdResult.Value;
            }

            var dealResult = DealEntity.Create(clientIdResult.Value, propertyIdResult.Value, bookingIdObj, details);
            if (dealResult.IsFailure)
            {
                return Result.Failure<DealEntity>(dealResult.Error);
            }

            return await _dealRepository.AddAsync(dealResult.Value);
        }

        public async Task<Result> ConfirmAsync(Guid dealId)
        {
            var dealResult = await GetByIdAsync(dealId);
            if (dealResult.IsFailure)
            {
                return Result.Failure(dealResult.Error);
            }

            var deal = dealResult.Value;
            if (!deal.Status.CanTransitionTo(DealStatus.Confirmed))
            {
                return Result.Failure($"Cannot transition deal from {deal.Status.Name} to Confirmed status");
            }

            deal.Confirm();
            return await _dealRepository.UpdateAsync(deal);
        }

        public async Task<Result> CompleteAsync(Guid dealId)
        {
            var dealResult = await GetByIdAsync(dealId);
            if (dealResult.IsFailure)
            {
                return Result.Failure(dealResult.Error);
            }

            var deal = dealResult.Value;
            if (!deal.Status.CanTransitionTo(DealStatus.Completed))
            {
                return Result.Failure($"Cannot transition deal from {deal.Status.Name} to Completed status");
            }

            deal.Complete();
            return await _dealRepository.UpdateAsync(deal);
        }

        public async Task<Result> CancelAsync(Guid dealId)
        {
            var dealResult = await GetByIdAsync(dealId);
            if (dealResult.IsFailure)
            {
                return Result.Failure(dealResult.Error);
            }

            var deal = dealResult.Value;
            if (!deal.Status.CanTransitionTo(DealStatus.Cancelled))
            {
                return Result.Failure($"Cannot transition deal from {deal.Status.Name} to Cancelled status");
            }

            deal.Cancel();
            return await _dealRepository.UpdateAsync(deal);
        }

        public async Task<Result> UpdateAsync(DealEntity dealEntity)
        {
            return await _dealRepository.UpdateAsync(dealEntity);
        }

        public async Task<Result> DeleteAsync(Guid dealId)
        {
            var idResult = DealId.Create(dealId);
            if (idResult.IsFailure)
            {
                return Result.Failure($"Invalid deal ID: {dealId}");
            }

            return await _dealRepository.DeleteAsync(idResult.Value);
        }

        public async Task<bool> ExistsAsync(Guid dealId)
        {
            var idResult = DealId.Create(dealId);
            if (idResult.IsFailure)
            {
                return false;
            }

            return await _dealRepository.ExistsAsync(idResult.Value);
        }
    }
}