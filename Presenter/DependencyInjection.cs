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
using UseCases.Buyer;
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
using UseCases.Property.Queries;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.Property.Queries.SearchPropertiesQuery;
using UseCases.Seller;
using UseCases.Services;
using UseCases.UseCases.DTO.Property;

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
            services.AddScoped<ICommandHandler<CreateDealCommand, DealEntity>, CreateDealCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBuyerCommand, BuyerEntity>, CreateBuyerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateSellerCommand, SellerEntity>, CreateSellerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBookingCommand, Guid>, CreateBookingCommandHandler>();
            services.AddScoped<ICommandHandler<ConfirmBookingCommand>, ConfirmBookingCommandHandler>();
            services.AddScoped<ICommandHandler<CancelBookingCommand>, CancelBookingCommandHandler>();
            services
                .AddScoped<ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>,
                    CreateCompleteDealCommandHandler>();
            services.AddScoped<ICommandHandler<ConfirmDealCommand>, ConfirmDealCommandHandler>();
            services.AddScoped<ICommandHandler<CompleteDealCommand>, CompleteDealCommandHandler>();
            services.AddScoped<ICommandHandler<CancelDealCommand>, CancelDealCommandHandler>();

            // Регистрация Query Handlers
            services.AddScoped<IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>>, GetPropertyByIdQueryHandler>();
            services.AddScoped<IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>>, SearchPropertiesQueryHandler>();

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
            services.AddScoped<IBookingService, BookingService>();

            return services;
        }
    }
}