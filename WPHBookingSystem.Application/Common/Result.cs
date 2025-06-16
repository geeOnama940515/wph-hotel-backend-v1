using System.Collections.Generic;

namespace WPHBookingSystem.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static Result<T> Success(T data, string? message = null, int statusCode = 200)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static Result<T> Failure(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }

    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static Result Success(string? message = null, int statusCode = 200)
        {
            return new Result
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static Result Failure(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new Result
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
} 