using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Inmobiliaria.Data
{
    public class MySqlConnectionFactory : IMySqlConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
