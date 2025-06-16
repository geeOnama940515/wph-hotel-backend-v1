using System.Collections.Generic;

namespace WPHBookingSystem.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

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

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static Result Success(string? message = null, int statusCode = 200)
        {
            return new Result
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }

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