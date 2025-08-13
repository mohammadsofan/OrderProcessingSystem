using OrderProcessingSystem.Api.Enums;

namespace OrderProcessingSystem.Api.Wrappers
{
    public class ServiceResult<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public ServiceResultStatus Status { get; set; }

        public static ServiceResult<T> Success(T data, string? message = null)
        {
            return new ServiceResult<T>
            {
                Data = data,
                IsSuccess = true,
                Status = ServiceResultStatus.Success,
                Message = message
            };
        }
        public static ServiceResult<T> Failure(string message, ServiceResultStatus status = ServiceResultStatus.InternalServerError)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Status = status,
                Message = message
            };
        }
    }
    public class ServiceResult
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public ServiceResultStatus Status { get; set; }
        public static ServiceResult Success(string? message = null)
        {
            return new ServiceResult
            {
                IsSuccess = true,
                Status = ServiceResultStatus.Success,
                Message = message
            };
        }
        public static ServiceResult Failure(string message, ServiceResultStatus status = ServiceResultStatus.InternalServerError)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Status = status,
                Message = message
            };
        }
    }
}