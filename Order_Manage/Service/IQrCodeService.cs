namespace Order_Manage.Service
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string content);
    }
}
