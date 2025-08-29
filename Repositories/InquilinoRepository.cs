using Inmobiliaria.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using Inmobiliaria.Data;

namespace Inmobiliaria.Repositories
{
    public class InquilinoRepository : IInquilinoRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;
        public InquilinoRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Inquilino>> GetAllAsync()
        {
            var list = new List<Inquilino>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "SELECT * FROM inquilinos";
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapInquilino(reader));
            }
            return list;
        }

        public async Task<Inquilino?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "SELECT * FROM inquilinos WHERE id=@id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapInquilino(reader);
            return null;
        }

        public async Task<long> CreateAsync(Inquilino i)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = @"INSERT INTO inquilinos (dni, apellido, nombre, telefono, email, direccion, created_at, updated_at, fecha_eliminacion) 
                        VALUES (@dni, @apellido, @nombre, @telefono, @email, @direccion, @created_at, @updated_at, @fecha_eliminacion); 
                        SELECT LAST_INSERT_ID();";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", i.Dni);
            cmd.Parameters.AddWithValue("@apellido", i.Apellido);
            cmd.Parameters.AddWithValue("@nombre", i.Nombre);
            cmd.Parameters.AddWithValue("@telefono", i.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", i.Email);
            cmd.Parameters.AddWithValue("@direccion", i.Direccion);
            cmd.Parameters.AddWithValue("@created_at", i.CreatedAt);
            cmd.Parameters.AddWithValue("@updated_at", i.UpdatedAt);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", i.FechaEliminacion ?? (object)DBNull.Value);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Inquilino i)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = @"UPDATE inquilinos SET dni=@dni, apellido=@apellido, nombre=@nombre, telefono=@telefono, email=@email, direccion=@direccion, updated_at=@updated_at, fecha_eliminacion=@fecha_eliminacion WHERE id=@id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", i.Id);
            cmd.Parameters.AddWithValue("@dni", i.Dni);
            cmd.Parameters.AddWithValue("@apellido", i.Apellido);
            cmd.Parameters.AddWithValue("@nombre", i.Nombre);
            cmd.Parameters.AddWithValue("@telefono", i.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", i.Email);
            cmd.Parameters.AddWithValue("@direccion", i.Direccion);
            cmd.Parameters.AddWithValue("@updated_at", i.UpdatedAt);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", i.FechaEliminacion ?? (object)DBNull.Value);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "DELETE FROM inquilinos WHERE id=@id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<(Inquilino?, Propietario?)> GetInquilinoAndPropietarioAsync(string dni)
        {
            Inquilino? inquilino = null;
            Propietario? propietario = null;
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            string sql = @"SELECT i.*, p.id AS PropietarioId, p.DNI AS PropietarioDNI, p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido 
                        FROM inquilinos i LEFT JOIN propietarios p ON i.DNI = p.DNI 
                        WHERE i.DNI = @dni";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", dni);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                inquilino = MapInquilino(reader);
                if (!reader.IsDBNull(reader.GetOrdinal("PropietarioId")))
                {
                    propietario = new Propietario
                    {
                        Id = reader.GetInt32("PropietarioId"),
                        Dni = reader.GetString("PropietarioDNI"),
                        Nombre = reader.GetString("PropietarioNombre"),
                        Apellido = reader.GetString("PropietarioApellido")
                    };
                }
            }
            return (inquilino, propietario);
        }

        private static Inquilino MapInquilino(MySqlDataReader reader) => new()
        {
            Id = reader.GetInt32("id"),
            Dni = reader.GetString("dni"),
            Apellido = reader.GetString("apellido"),
            Nombre = reader.GetString("nombre"),
            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
            Email = reader.GetString("email"),
            Direccion = reader.GetString("direccion"),
            CreatedAt = reader.GetDateTime("created_at"),
            UpdatedAt = reader.GetDateTime("updated_at"),
            FechaEliminacion = reader.IsDBNull(reader.GetOrdinal("fecha_eliminacion")) ? null : reader.GetDateTime("fecha_eliminacion")
        };
    }
}
