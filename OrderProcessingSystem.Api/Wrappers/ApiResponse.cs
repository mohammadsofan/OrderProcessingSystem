namespace OrderProcessingSystem.Api.Wrappers
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public IList<string>? Errors { get; set; }
        public ApiResponse(bool isSuccess, string? message, object? data = null, IList<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Errors = errors;
        }
        public static ApiResponse Success(string? message, object? data = null)
        {
            return new ApiResponse(true,message,data);
        }
        public static ApiResponse Failure(string? message, IList<string>? errors = null)
        {
            return new ApiResponse(false, message,null, errors);
        }
    }
}
