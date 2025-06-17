using Microsoft.AspNetCore.Mvc;
using WPHBookingSystem.Application.Common;

namespace WPHBookingSystem.WebUI.Extensions
{
    /// <summary>
    /// Extension methods for ASP.NET Core controllers to provide standardized API responses.
    /// 
    /// These extensions ensure consistent response format across all API endpoints,
    /// following REST API best practices with standardized success/error handling.
    /// All responses include a success flag, message, and appropriate HTTP status codes.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Creates a standardized API response from a Result<T> object.
        /// 
        /// This method automatically maps the Result pattern to HTTP responses,
        /// ensuring consistent error handling and success responses across the API.
        /// </summary>
        /// <typeparam name="T">The type of data being returned</typeparam>
        /// <param name="controller">The controller instance</param>
        /// <param name="result">The Result object containing success status, message, and data</param>
        /// <returns>An IActionResult with standardized response format</returns>
        public static IActionResult CreateResponse<T>(this ControllerBase controller, Result<T> result)
        {
            var response = new
            {
                success = result.IsSuccess,
                message = result.Message,
                data = result.IsSuccess ? result.Data : (object?)null,
                errors = result.Errors
            };

            return controller.StatusCode(result.StatusCode, response);
        }

        /// <summary>
        /// Creates a standardized API response from a Result object (without data).
        /// 
        /// Used for operations that don't return data but need to communicate success/failure status.
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="result">The Result object containing success status and message</param>
        /// <returns>An IActionResult with standardized response format</returns>
        public static IActionResult CreateResponse(this ControllerBase controller, Result result)
        {
            var response = new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors
            };

            return controller.StatusCode(result.StatusCode, response);
        }

        /// <summary>
        /// Creates a standardized API response with custom status code, message, and data.
        /// 
        /// Provides flexibility for custom response scenarios while maintaining consistent format.
        /// </summary>
        /// <typeparam name="T">The type of data being returned</typeparam>
        /// <param name="controller">The controller instance</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="message">Response message</param>
        /// <param name="data">Response data</param>
        /// <returns>An IActionResult with standardized response format</returns>
        public static IActionResult CreateResponse<T>(this ControllerBase controller, int statusCode, string message, T data)
        {
            var response = new
            {
                success = statusCode >= 200 && statusCode < 300,
                message,
                data,
                errors = (object)null
            };

            return controller.StatusCode(statusCode, response);
        }

        /// <summary>
        /// Creates a standardized API response with custom status code and message (no data).
        /// 
        /// Used for simple success/error responses without additional data payload.
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="message">Response message</param>
        /// <returns>An IActionResult with standardized response format</returns>
        public static IActionResult CreateResponse(this ControllerBase controller, int statusCode, string message)
        {
            var response = new
            {
                success = statusCode >= 200 && statusCode < 300,
                message,
                errors = (object)null
            };

            return controller.StatusCode(statusCode, response);
        }
    }
} 