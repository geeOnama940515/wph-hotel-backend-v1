using System.Collections.Generic;

namespace WPHBookingSystem.Application.Common
{
    /// <summary>
    /// Generic result wrapper that provides a consistent way to handle operation outcomes.
    /// This pattern ensures uniform error handling and response formatting across the application.
    /// 
    /// The Result pattern encapsulates both success and failure states, making it easier
    /// to handle errors without throwing exceptions for business logic failures.
    /// </summary>
    /// <typeparam name="T">The type of data returned on successful operations.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation.
        /// This will be null if the operation failed.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets a human-readable message describing the operation result.
        /// This can be a success message or an error description.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a list of validation errors or detailed error messages.
        /// This is typically populated when the operation fails due to validation issues.
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code that should be returned to the client.
        /// This helps the API layer determine the appropriate HTTP response code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Creates a successful result with the specified data.
        /// </summary>
        /// <param name="data">The data to be returned.</param>
        /// <param name="message">Optional success message.</param>
        /// <param name="statusCode">HTTP status code (default: 200 OK).</param>
        /// <returns>A Result instance representing a successful operation.</returns>
        public static Result<T> Success(T data, string? message = null, int statusCode = 200)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a failed result with the specified error information.
        /// </summary>
        /// <param name="message">The error message describing what went wrong.</param>
        /// <param name="statusCode">HTTP status code (default: 400 Bad Request).</param>
        /// <param name="errors">Optional list of detailed error messages.</param>
        /// <returns>A Result instance representing a failed operation.</returns>
        public static Result<T> Failure(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Non-generic result wrapper for operations that don't return data.
    /// This is used for operations that only need to indicate success or failure
    /// without returning any specific data.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets a human-readable message describing the operation result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a list of validation errors or detailed error messages.
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code that should be returned to the client.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Creates a successful result without data.
        /// </summary>
        /// <param name="message">Optional success message.</param>
        /// <param name="statusCode">HTTP status code (default: 200 OK).</param>
        /// <returns>A Result instance representing a successful operation.</returns>
        public static Result Success(string? message = null, int statusCode = 200)
        {
            return new Result
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a failed result without data.
        /// </summary>
        /// <param name="message">The error message describing what went wrong.</param>
        /// <param name="statusCode">HTTP status code (default: 400 Bad Request).</param>
        /// <param name="errors">Optional list of detailed error messages.</param>
        /// <returns>A Result instance representing a failed operation.</returns>
        public static Result Failure(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new Result
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
} 