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
    /// <summary>
    /// Static class responsible for configuring and registering all application layer services
    /// with the dependency injection container. This class follows the extension method pattern
    /// to provide a clean and organized way to register application services.
    /// 
    /// The dependency injection setup ensures that:
    /// - All use cases are properly registered with their dependencies
    /// - The facade service is registered with all required use case dependencies
    /// - Services are registered with appropriate lifetimes (Scoped in this case)
    /// - The application layer is properly integrated with the infrastructure layer
    /// </summary>
    public static class ApplicationDependencyInjection
    {
        /// <summary>
        /// Registers all application layer services with the dependency injection container.
        /// This extension method should be called during application startup to configure
        /// the application layer dependencies.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection ApplicationDependencyiInjection(this IServiceCollection services)
        {
            // Register Booking Use Cases
            // These use cases handle all booking-related business operations
            services.AddScoped<CreateBookingUseCase>();
            services.AddScoped<UpdateBookingDatesUseCase>();
            services.AddScoped<UpdateBookingStatusUseCase>();
            services.AddScoped<CancelBookingUseCase>();
            services.AddScoped<GetUserBookingsUseCase>();

            // Register Room Use Cases
            // These use cases handle all room-related business operations
            services.AddScoped<CreateRoomUseCase>();
            services.AddScoped<UpdateRoomUseCase>();
            services.AddScoped<UpdateRoomWithImagesUseCase>();
            services.AddScoped<UpdateRoomStatusUseCase>();
            services.AddScoped<GetRoomByIdUseCase>();
            services.AddScoped<GetAllRoomsUseCase>();
            services.AddScoped<DeleteRoomUseCase>();
            services.AddScoped<CheckRoomAvailabilityUseCase>();
            services.AddScoped<GetRoomOccupancyRateUseCase>();
            services.AddScoped<GetRoomRevenueUseCase>();
            services.AddScoped<UploadRoomImagesUseCase>();
            services.AddScoped<ViewBookingByTokenUseCase>();

            // Register Facade Service
            // The BookingSystemFacade acts as a unified entry point for all booking system operations
            // It orchestrates the use cases and provides a simplified interface to the presentation layer
            services.AddScoped<IBookingSystemFacade, BookingSystemFacade>();

            return services;
        }
    }
}
