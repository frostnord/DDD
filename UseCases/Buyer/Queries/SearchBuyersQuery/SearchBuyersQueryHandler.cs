using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UseCases.UseCases.DTO.Buyer;
using UseCases.Interfaces.Queries;
using UseCases.Buyer.Queries.SearchBuyersQuery;
using UseCases.Interfaces.Repositories;

namespace UseCases.Buyer.Queries.SearchBuyersQuery
{
    public class SearchBuyersQueryHandler : IQueryHandler<SearchBuyersQuery, Result<SearchBuyersQueryResponse>>
    {
        private readonly IBuyerRepository _buyerRepository;

        public SearchBuyersQueryHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        public async Task<Result<SearchBuyersQueryResponse>> HandleAsync(SearchBuyersQuery query)
        {
            var buyersResult = await _buyerRepository.GetAllAsync();
            if (buyersResult.IsFailure)
            {
                return Result.Failure<SearchBuyersQueryResponse>(buyersResult.Error);
            }

            var buyers = buyersResult.Value;
            var totalCount = buyers.Count();
            var pageSize = query.PageSize;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var page = Math.Max(1, Math.Min(query.Page, totalPages > 0 ? totalPages : 1));
            var skip = (page - 1) * pageSize;

            var pagedBuyers = buyers.Skip(skip).Take(pageSize).ToList();

            // Используем DateTime.UtcNow, так как RegistrationDate отсутствует в BuyerEntity
            var dtos = pagedBuyers.Select(b => new BuyerDto(b.Id.Value, b.ClientId.Value, DateTime.UtcNow)).ToList();

            var response = new SearchBuyersQueryResponse(dtos, totalCount, pageSize, totalPages);

            return Result.Success(response);
        }
    }
}