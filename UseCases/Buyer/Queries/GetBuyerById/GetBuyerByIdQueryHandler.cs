using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer.VO;
using UseCases.UseCases.DTO.Buyer;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;

namespace UseCases.Buyer.Queries.GetBuyerById;

public class GetBuyerByIdQueryHandler : IQueryHandler<GetBuyerByIdQuery, Result<BuyerDto>>
{
    private readonly IBuyerRepository _buyerRepository;

    public GetBuyerByIdQueryHandler(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }

    public async Task<Result<BuyerDto>> HandleAsync(GetBuyerByIdQuery query)
    {
        var buyerId = BuyerId.Create(query.BuyerId);
        if (buyerId.IsFailure)
            return Result.Failure<BuyerDto>(buyerId.Error);

        var buyerResult = await _buyerRepository.GetByIdAsync(buyerId.Value);
        if (buyerResult.IsFailure)
        {
            return Result.Failure<BuyerDto>(buyerResult.Error);
        }

        var buyer = buyerResult.Value;
        // Используем DateTime.UtcNow, так как RegistrationDate отсутствует в BuyerEntity
        var dto = new BuyerDto(buyer.Id.Value, buyer.ClientId.Value, DateTime.UtcNow);
        return Result.Success(dto);
    }
}