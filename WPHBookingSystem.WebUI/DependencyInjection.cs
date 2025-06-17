namespace WPHBookingSystem.WebUI
{
    /// <summary>
    /// Web UI layer dependency injection configuration.
    /// 
    /// This class provides extension methods for registering Web UI specific services.
    /// Currently minimal as most services are registered in the Application and Infrastructure layers.
    /// Can be extended to include Web UI specific services like:
    /// - Custom middleware
    /// - API versioning
    /// - CORS policies
    /// - Custom authentication schemes
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers Web UI specific services with the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to register services with</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection WebUIServices(this IServiceCollection services)
        {
            // Web UI specific services can be registered here
            // Currently empty as most services are handled by Application and Infrastructure layers
            return services;
        }
    }
}
