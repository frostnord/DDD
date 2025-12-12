using Domain.Booking;
using Domain.Customers.Buyer;
using Domain.Customers.Seller;
using Domain.Deal;
using Domain.Property;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using UseCases.Booking.Commands;
using UseCases.Clients.Commands;
using UseCases.Handlers;
using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;
using UseCases.Services;

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
            services.AddScoped<ICommandHandler<CreatePropertyCommand, PropertyEntity>, CreatePropertyCommandHandler>();
            services.AddScoped<ICommandHandler<CreateDealCommand, DealEntity>, CreateDealCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBuyerCommand, BuyerEntity>, CreateBuyerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateSellerCommand, SellerEntity>, CreateSellerCommandHandler>();
            services.AddScoped<ICommandHandler<CreateBookingCommand, BookingEntity>, CreateBookingCommandHandler>();
            services
                .AddScoped<ICommandHandler<CreateCompleteDealCommand, CompletedDeal>,
                    CreateCompleteDealCommandHandler>();

            // Регистрация репозиториев
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IDealRepository, DealRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ICompletedDealRepository, CompletedDealRepository>();
            services.AddScoped<IAgencyRepository, AgencyRepository>();

            // Регистрация сервисов
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<IBuyerService, BuyerService>();

            return services;
        }
    }
}