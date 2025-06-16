using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Application.Services;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Application.UseCases.Rooms;

namespace WPHBookingSystem.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection ApplicationDependencyiInjection(this IServiceCollection services)
        {
            // Register Use Cases
            services.AddScoped<CreateBookingUseCase>();
            services.AddScoped<UpdateBookingDatesUseCase>();
            services.AddScoped<UpdateBookingStatusUseCase>();
            services.AddScoped<CancelBookingUseCase>();
            services.AddScoped<GetUserBookingsUseCase>();

            services.AddScoped<CreateRoomUseCase>();
            services.AddScoped<UpdateRoomUseCase>();
            services.AddScoped<UpdateRoomStatusUseCase>();
            services.AddScoped<GetRoomByIdUseCase>();
            services.AddScoped<GetAllRoomsUseCase>();
            services.AddScoped<DeleteRoomUseCase>();
            services.AddScoped<CheckRoomAvailabilityUseCase>();
            services.AddScoped<GetRoomOccupancyRateUseCase>();
            services.AddScoped<GetRoomRevenueUseCase>();

            // Register Facade
            services.AddScoped<IBookingSystemFacade, BookingSystemFacade>();

            return services;
        }
    }
}
