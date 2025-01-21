namespace Order_Manage.Exceptions
{
    public enum ErrorCode
    {
        //User
        EMAIL_NOT_VALID = 3000,
        USER_NOT_LOGIN = 3001,
        LOGIN_NOT_VALID = 3004,
        ROLE_NOT_VALID = 3005,
        EMAIL_WAS_REGISTER = 3006,
        PASS_WORD_NULL = 3007,
        CANNOT_UPDATE_ACCOUNT = 3008,
        TOKEN_INVALID = 3009,
        USER_NOT_AUTHORIZED = 3010,

        //Order
        ORDER_DETAILS_MISSING = 4000,
        ORDER_CREATION_FAILED = 4001,
        ORDER_HISTORY_FAILED = 4002,
        ORDER_NOT_FOUND = 4003,
    }

    public static class ErrorCodeExtensions
    {
        public static string GetMessage(this ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.EMAIL_NOT_VALID => "Email không hợp lệ",
                ErrorCode.USER_NOT_LOGIN => "Hãy đăng nhập để tiếp tục",
                ErrorCode.LOGIN_NOT_VALID => "Thông tin đăng nhập không hợp lệ!",
                ErrorCode.ROLE_NOT_VALID => "Không tìm thấy role này",
                ErrorCode.EMAIL_WAS_REGISTER => "Email này đã được sử dụng",
                ErrorCode.PASS_WORD_NULL => "Không được để mật khẩu trống",
                ErrorCode.CANNOT_UPDATE_ACCOUNT => "Không thể update account",
                ErrorCode.TOKEN_INVALID => "Token không hợp lệ",
                ErrorCode.USER_NOT_AUTHORIZED=> "ORDER_CREATION_FAILED",


                ErrorCode.ORDER_CREATION_FAILED => "ORDER_CREATION_FAILED",
                ErrorCode.ORDER_DETAILS_MISSING => "ORDER_DETAILS_MISSING",
                ErrorCode.ORDER_HISTORY_FAILED => "ORDER_HISTORY_FAILED",
                ErrorCode.ORDER_NOT_FOUND => "ORDER_NOT_FOUND",
                _ => "Lỗi không xác định",
            };
        }
    }
}
