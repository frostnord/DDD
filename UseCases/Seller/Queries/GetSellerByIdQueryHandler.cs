using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Seller.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.DTO.Seller;

namespace UseCases.Seller.Queries;

public class GetSellerByIdQueryHandler : IQueryHandler<GetSellerByIdQuery, Result<SellerDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSellerByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SellerDto>> HandleAsync(GetSellerByIdQuery query)
    {
        var sellerIdResult = SellerId.Create(query.SellerId);
        if (sellerIdResult.IsFailure)
        {
            return Result.Failure<SellerDto>(sellerIdResult.Error);
        }

        var sellerResult = await _unitOfWork.Sellers.GetByIdAsync(sellerIdResult.Value);
        if (sellerResult.IsFailure)
        {
            return Result.Failure<SellerDto>(sellerResult.Error);
        }

        var seller = sellerResult.Value;
        var sellerDto = new SellerDto(
            seller.Id.Value,
            seller.ClientId.Value,
            seller.RegisteredAt
        );

        return Result.Success(sellerDto);
    }
}