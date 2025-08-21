using MySqlConnector;

namespace Inmobiliaria.Data
{
    public interface IMySqlConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}