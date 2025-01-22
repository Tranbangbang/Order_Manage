using Order_Manage.Models;

namespace Order_Manage.Repository
{
    public interface IAccountRepository
    {
        Account FindByEmail(string email);
        int InsertViaCode(ViaCode viaCode);
        ViaCode GetViaCodeByEmail(string email);


    }
}
