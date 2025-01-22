using Order_Manage.Common.Constants.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Models;

namespace Order_Manage.Service
{
    public interface IAccountService
    {
        ApiResponse<string> handleSendCodeToMail(ViaCodeRequest request);
        int generateCode();
        int sendCodeToEmail(string to, string subject, string content);
        ViaCode insertCode(int code, string email);
        
    }
}
