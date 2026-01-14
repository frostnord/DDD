using System;
using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Buyer;
using Domain.Customers.Seller;
using Domain.Deal;
using Domain.Property;
using Domain.Property.VO;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Booking.Commands;
using UseCases.Booking.Commands.CancelBooking;
using UseCases.Booking.Commands.ConfirmBooking;
using UseCases.Booking.Commands.CreateBooking;
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
using UseCases.Deal;
using UseCases.Deal.Commands;
using UseCases.Interfaces;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;
using UseCases.Property;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Commands.DeleteProperty;
using UseCases.Property.Commands.UpdateProperty;
using UseCases.Property.Queries;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.Seller;
using UseCases.Services;
using UseCases.UseCases.DTO.Property;
using System.Collections.Generic;
using UseCases.DTO.Seller;
using UseCases.Seller.Commands;
using UseCases.Seller.Queries;

namespace Presenter
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            string connectionString)
        {
            // Регистрация DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Регистрация Use Case Handlers
            services.AddScoped<ICommandHandler<CreatePropertyCommand, Guid>, CreatePropertyCommandHandler>();
            services.AddScoped<ICommandHandler<UpdatePropertyCommand>, UpdatePropertyCommandHandler>();
            services.AddScoped<ICommandHandler<DeletePropertyCommand>, DeletePropertyCommandHandler>();

            services.AddScoped<ICommandHandler<CreateDealCommand, DealEntity>, CreateDealCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBuyerCommand, Guid>, CreateBuyerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateSellerCommand, Guid>, CreateSellerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBookingCommand, Guid>, CreateBookingCommandHandler>();
            services.AddScoped<ICommandHandler<ConfirmBookingCommand>, ConfirmBookingCommandHandler>();
            services.AddScoped<ICommandHandler<CancelBookingCommand>, CancelBookingCommandHandler>();
            services
                .AddScoped<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>,
                    CreateCompleteDealCommandHandler>();
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
            
            // Регистрация обработчиков запросов для клиентов
            services.AddScoped<IQueryHandler<GetClientByIdQuery, Result<Domain.Customers.Client.ClientEntity>>, GetClientByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllClientsQuery, Result<IEnumerable<Domain.Customers.Client.ClientEntity>>>, GetAllClientsQueryHandler>();

            // Регистрация обработчиков запросов для продавцов
            services.AddScoped<IQueryHandler<GetSellerByIdQuery, Result<SellerDto>>, GetSellerByIdQueryHandler>();
            services.AddScoped<IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>>, SearchSellersQueryHandler>();

            // Регистрация репозиториев
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IDealRepository, DealRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ICompletedDealRepository, CompletedDealRepository>();

            // Регистрация сервисов
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<IBuyerService, BuyerService>();
            services.AddScoped<IDealService, DealService>();
            services.AddScoped<ICompletedDealService, CompletedDealService>();

            return services;
        }
    }
}