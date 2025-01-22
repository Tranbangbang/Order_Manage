using Order_Manage.Models;

namespace Order_Manage.Repository
{
    public interface IMessageRepository
    {
        int SaveMessageAsync(Message message);
        IEnumerable<Message> GetChatHistoryAsync(string senderId, string receiverId);
    }
}
