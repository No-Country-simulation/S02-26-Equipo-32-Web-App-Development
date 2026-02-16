using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ApiResponse(int statusCode, string message, T? data = default)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> Success(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>(200, message, data);
        }

        public static ApiResponse<T> Created(T data, string message = "Recurso creado exitosamente")
        {
            return new ApiResponse<T>(201, message, data);
        }

        public static ApiResponse<T> Error(int statusCode, string message)
        {
            return new ApiResponse<T>(statusCode, message);
        }
    }
}
