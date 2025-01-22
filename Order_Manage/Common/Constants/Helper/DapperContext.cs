using MySqlConnector;
using System.Data;

namespace Order_Manage.Common.Constants.Helper
{
    public class DapperContext(IConfiguration configuration)
    {
        private readonly string? _connectionString = configuration.GetConnectionString("DefaultConnection");
        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}
