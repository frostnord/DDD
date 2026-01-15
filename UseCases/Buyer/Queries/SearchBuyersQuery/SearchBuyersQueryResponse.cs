using System.Collections.Generic;
using UseCases.UseCases.DTO.Buyer;

namespace UseCases.Buyer.Queries.SearchBuyersQuery;

public record SearchBuyersQueryResponse(List<BuyerDto> Items, int TotalCount, int PageSize, int TotalPages);