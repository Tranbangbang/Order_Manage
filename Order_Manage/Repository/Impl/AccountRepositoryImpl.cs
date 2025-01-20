using Dapper;
using Order_Manage.Dto.Helper;
using Order_Manage.Models;
using Order_Manage.XML;

namespace Order_Manage.Repository.Impl
{
    public class AccountRepositoryImpl : IAccountRepository
    {
        private readonly DapperContext _context;
        private readonly QueryLoader _queryLoader;

        public AccountRepositoryImpl(DapperContext context, QueryLoader queryLoader)
        {
            _context = context;
            _queryLoader = queryLoader;
        }

        public Account FindByEmail(string email)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("get-acc-by-Email", out var query))
                throw new KeyNotFoundException("Query 'get-acc-by-Email' not found in XML file");
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = connection.QueryFirstOrDefault<Account>(
                    query,
                    new { Email = email },
                    transaction: transaction
                );
                transaction.Commit(); 
                return result;
            }
            catch
            {
                transaction.Rollback(); 
                throw;
            }
        }

        public int GetViaCodeByEmail(string email)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("get-viaCode-by-Email", out var query))
                throw new KeyNotFoundException("Query 'get-viacode-by-Email' not found in XML file");
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = connection.QueryFirstOrDefault<ViaCode>(
                    query,
                    new { Email = email },
                    transaction: transaction
                );
                transaction.Commit();
                return 1;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public int InsertViaCode(ViaCode viaCode)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("insert-via-code", out var query))
                throw new KeyNotFoundException("Query 'insert-via-code' not found in XML file");

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = connection.Execute(
                    query,
                    new
                    {
                        viaCode.viaCode,
                        viaCode.email,
                        createAt = viaCode.createAt
                    },
                    transaction: transaction
                );
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
