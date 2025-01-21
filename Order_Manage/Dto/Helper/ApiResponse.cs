using Order_Manage.Exceptions;

namespace Order_Manage.Dto.Helper
{
    public class ApiResponse<T>
    {
        public int Code { get; set; } = 200;
        public string? Message { get; set; }
        public T? Data { get; set; }


        public ApiResponse()
        {
        }
        public ApiResponse(T data)
        {
            Data = data;
        }

        private ApiResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
        private ApiResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T>(200, message, data);
        }
        public static ApiResponse<T> Error(int code, string message)
        {
            return new ApiResponse<T>(code, message);
        }

        public static ApiResponse<T> Error(ErrorCode errorCode)
        {
            return new ApiResponse<T>((int)errorCode, errorCode.GetMessage());
        }
    }
}
