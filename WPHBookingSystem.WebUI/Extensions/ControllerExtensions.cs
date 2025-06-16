using Microsoft.AspNetCore.Mvc;
using WPHBookingSystem.Application.Common;

namespace WPHBookingSystem.WebUI.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult CreateResponse<T>(this ControllerBase controller, Result<T> result)
        {
            return controller.StatusCode(result.StatusCode, new
            {
                success = result.IsSuccess,
                message = result.Message,
                data = result.Data,
                errors = result.Errors
            });
        }

        public static IActionResult CreateResponse(this ControllerBase controller, Result result)
        {
            return controller.StatusCode(result.StatusCode, new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors
            });
        }

        public static IActionResult CreateResponse<T>(this ControllerBase controller, int statusCode, string message, T data)
        {
            return controller.StatusCode(statusCode, new
            {
                success = statusCode >= 200 && statusCode < 300,
                message,
                data
            });
        }

        public static IActionResult CreateResponse(this ControllerBase controller, int statusCode, string message)
        {
            return controller.StatusCode(statusCode, new
            {
                success = statusCode >= 200 && statusCode < 300,
                message
            });
        }
    }
} 