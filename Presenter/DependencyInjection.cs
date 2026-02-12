using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Buyer;
using Domain.Customers.Seller;
using Domain.Deal;
using Domain.Property;
using Domain.Property.VO;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Queries.GetBookingById;
using UseCases.Booking.Queries.SearchBookingsQuery;
using UseCases.Buyer;
using UseCases.Buyer.Commands.CreateBuyer;
using UseCases.Client.Commands;
using UseCases.Client.Commands.CreateClient;
using UseCases.Client.Commands.DeleteClient;
using UseCases.Client.Commands.UpdateClient;
using UseCases.Client.Queries;
using UseCases.Client.Queries.GetAllClient;
using UseCases.Client.Queries.GetClientById;
using UseCases.CompleteDeal;
using UseCases.CompleteDeal.Commands.CreateCompliteDealCommand;
using UseCases.CompleteDeal.Commands.DeleteCompletedDeal;
using UseCases.CompleteDeal.Queries.GetAllCompletedDeals;
using UseCases.CompleteDeal.Queries.GetCompletedDealById;
using UseCases.CompleteDeal.Queries.GetCompletedDealsByClientId;
using UseCases.CompleteDeal.Queries.GetCompletedDealsByPropertyId;
using UseCases.Deal;
using UseCases.Deal.Commands;
using UseCases.Deal.Queries.GetDealById;
using UseCases.Deal.Queries.SearchDealsQuery;
using UseCases.DTO.Seller;
using UseCases.Interfaces;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.Property;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Commands.DeleteProperty;
using UseCases.Property.Commands.UpdateProperty;
using UseCases.Property.Queries;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.Seller.Commands;
using UseCases.Seller.Queries;
using UseCases.UseCases.DTO.Property;

namespace Presenter
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Регистрация Use Case Handlers
            services.AddScoped<ICommandHandler<CreatePropertyCommand, Guid>, CreatePropertyCommandHandler>();
            services.AddScoped<ICommandHandler<UpdatePropertyCommand>, UpdatePropertyCommandHandler>();
            services.AddScoped<ICommandHandler<DeletePropertyCommand>, DeletePropertyCommandHandler>();

            services.AddScoped<ICommandHandler<CreateDealCommand, Guid>, CreateDealCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBuyerCommand, Guid>, CreateBuyerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateSellerCommand, Guid>, CreateSellerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBookingCommand, Guid>, CreateBookingCommandHandler>();
            services.AddScoped<ICommandHandler<ConfirmBookingCommand>, ConfirmBookingCommandHandler>();
            services.AddScoped<ICommandHandler<CancelBookingCommand>, CancelBookingCommandHandler>();
            services
                .AddScoped<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>,
                    CreateCompleteDealCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteCompletedDealCommand>, DeleteCompletedDealCommandHandler>();
            services.AddScoped<ICommandHandler<ConfirmDealCommand>, ConfirmDealCommandHandler>();
            services.AddScoped<ICommandHandler<CompleteDealCommand>, CompleteDealCommandHandler>();
            services.AddScoped<ICommandHandler<CancelDealCommand>, CancelDealCommandHandler>();

            services.AddScoped<ICommandHandler<UpdateSellerCommand>, UpdateSellerCommandHandler>();
            
            // Регистрация обработчиков команд для клиентов
            services.AddScoped<ICommandHandler<CreateClientCommand, Domain.Customers.Client.ClientEntity>, CreateClientCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateClientCommand, Domain.Customers.Client.ClientEntity>, UpdateClientCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteClientCommand, Domain.Customers.Client.ClientEntity>, DeleteClientCommandHandler>();

            // Регистрация Query Handlers
            services.AddScoped<IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>>, GetPropertyByIdQueryHandler>();
            services.AddScoped<IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>>, SearchPropertiesQueryHandler>();
            services.AddScoped<IQueryHandler<GetBookingByIdQuery, Result<UseCases.UseCases.DTO.Booking.BookingDto>>, GetBookingByIdQueryHandler>();
            services.AddScoped<IQueryHandler<SearchBookingsQuery, Result<SearchBookingsQueryResponse>>, SearchBookingsQueryHandler>();
            services.AddScoped<IQueryHandler<GetDealByIdQuery, Result<UseCases.UseCases.DTO.Deal.DealDto>>, GetDealByIdQueryHandler>();
            services.AddScoped<IQueryHandler<SearchDealsQuery, Result<SearchDealsQueryResponse>>, SearchDealsQueryHandler>();
            services.AddScoped<IQueryHandler<GetCompletedDealByIdQuery, Result<UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto>>, GetCompletedDealByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllCompletedDealsQuery, Result<IEnumerable<UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto>>>, GetAllCompletedDealsQueryHandler>();
            services.AddScoped<IQueryHandler<GetCompletedDealsByClientIdQuery, Result<IEnumerable<UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto>>>, GetCompletedDealsByClientIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto>>>, GetCompletedDealsByPropertyIdQueryHandler>();
            
            // Регистрация обработчиков запросов для клиентов
            services.AddScoped<IQueryHandler<GetClientByIdQuery, Result<Domain.Customers.Client.ClientEntity>>, GetClientByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllClientsQuery, Result<IEnumerable<Domain.Customers.Client.ClientEntity>>>, GetAllClientsQueryHandler>();

            // Регистрация обработчиков запросов для продавцов
            services.AddScoped<IQueryHandler<GetSellerByIdQuery, Result<SellerDto>>, GetSellerByIdQueryHandler>();
            services.AddScoped<IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>>, SearchSellersQueryHandler>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
