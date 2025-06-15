using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Infrastructure.Repositories;

namespace WPHBookingSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InfrastructureInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRoomRepository,RoomRepository>();
            services.AddScoped<IBookingRepository,BookingRepository>();

            return services;
        }
    }
}
