using MySqlConnector;

namespace Inmobiliaria_.Net_Core.Models
{
    public abstract class RepositorioBase
    {
        protected readonly string connectionString;

        public RepositorioBase(IConfiguration configuration) => connectionString = configuration.GetConnectionString("DefaultConnection");

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}

