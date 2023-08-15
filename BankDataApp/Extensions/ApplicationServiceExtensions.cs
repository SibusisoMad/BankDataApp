using BankDataApp.Data;
using BankDataApp.Data.Repositories;
using BankDataApp.Helpers;
using BankDataApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace BankDataApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContextFactory<DataContext>(options => options.UseSqlServer(
                connectionString, sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();

                }), ServiceLifetime.Transient);
            return services;
        }

    }
}
