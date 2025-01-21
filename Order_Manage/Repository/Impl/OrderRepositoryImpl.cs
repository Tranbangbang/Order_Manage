using Dapper;
using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Response;
using Order_Manage.Models;
using Order_Manage.XML;

namespace Order_Manage.Repository.Impl
{
    public class OrderRepositoryImpl : IOrderRepository
    {
        private readonly DapperContext _context;
        private readonly QueryLoader _queryLoader;

        public OrderRepositoryImpl(DapperContext context, QueryLoader queryLoader)
        {
            _context = context;
            _queryLoader = queryLoader;
        }
        public int CreateOrder(Order order)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("insert-order", out var insertOrderQuery) ||
                !sql.TryGetValue("insert-order-detail", out var insertOrderDetailQuery))
                throw new KeyNotFoundException("Queries for inserting order or order details not found in XML file");
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var orderId = connection.QuerySingle<int>(insertOrderQuery,
                    new { order.OrderDate, order.AccountId },
                    transaction: transaction);
                if (order.OrderDetails != null)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        connection.Execute(insertOrderDetailQuery,
                            new { OrderId = orderId, detail.ProductId, detail.Quantity },
                            transaction: transaction);
                    }
                }
                transaction.Commit();
                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<OrderResponse> GetOrdersByUser(string accountId)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("get-user-orders", out var query))
                throw new KeyNotFoundException("Query 'get-user-orders' not found in XML file");

            using var connection = _context.CreateConnection();
            connection.Open();
            var orderDictionary = new Dictionary<int, OrderResponse>();

            var result = connection.Query<OrderResponse, OrderDetailResponse, ProductResponse, OrderResponse>(
                query,
                (order, detail, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.OrderId, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.OrderDetail = detail;
                        orderEntry.OrderDetail.ListProducts = new List<ProductResponse>();
                        orderDictionary.Add(order.OrderId, orderEntry);
                    }
                    orderEntry.OrderDetail.ListProducts.Add(product);
                    return orderEntry;
                },
                new { AccountId = accountId },
                splitOn: "OrderDetailId,ProductId"
            );

            return orderDictionary.Values.ToList();
        }
    }
}
