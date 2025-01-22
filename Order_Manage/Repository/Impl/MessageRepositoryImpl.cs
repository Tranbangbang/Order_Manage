using Dapper;
using Order_Manage.Models;
using Order_Manage.XML;
using Order_Manage.Common.Constants.Helper;

namespace Order_Manage.Repository.Impl
{
    public class MessageRepositoryImpl : IMessageRepository
    {
        private readonly DapperContext _context;
        private readonly QueryLoader _queryLoader;

        public MessageRepositoryImpl(DapperContext context, QueryLoader queryLoader)
        {
            _context = context;
            _queryLoader = queryLoader;
        }

        public IEnumerable<Message> GetChatHistoryAsync(string senderId, string receiverId)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("get-chat-history", out var getChatHistoryQuery))
                throw new KeyNotFoundException("Query 'get-chat-history' not found in XML file");

            using var connection = _context.CreateConnection();
            return connection.Query<Message>(getChatHistoryQuery, new
            {
                SenderId = senderId,
                ReceiverId = receiverId
            }).ToList();
        }

        public int SaveMessageAsync(Message message)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("insert-message", out var insertMessageQuery))
                throw new KeyNotFoundException("Query 'insert-message' not found in XML file");

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = connection.Execute(insertMessageQuery, new
                {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content
                }, transaction);

                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
