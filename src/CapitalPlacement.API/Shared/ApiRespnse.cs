namespace CapitalPlacement.API.Shared
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(string message, T data = default!)
        {
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> FromSuccess(string message, T data)
        {
            return new ApiResponse<T>(message, data);
        }
    }
}
