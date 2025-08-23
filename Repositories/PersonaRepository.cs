using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly IMySqlConnectionFactory _factory;

        public PersonaRepository(IMySqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<Persona>> GetAllAsync(CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"SELECT id, DNI, nombre, apellido, contacto FROM personas ORDER BY id DESC";
            using var cmd = new MySqlCommand(sql, conn);

            var list = new List<Persona>();
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                list.Add(MapPersona(reader));
            }
            return list;
        }

        public async Task<Persona?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"SELECT id, DNI, nombre, apellido, contacto FROM personas WHERE id=@id LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
                return MapPersona(reader);

            return null;
        }

        public async Task<int> CreateAsync(Persona p, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"
INSERT INTO personas (DNI, nombre, apellido, contacto)
VALUES (@DNI, @Nombre, @Apellido, @Contacto);
SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@DNI", p.DNI);
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@Contacto", p.Contacto);

            var result = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Persona p, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"
                                UPDATE personas 
                                SET DNI=@DNI, nombre=@Nombre, apellido=@Apellido, contacto=@Contacto
                                WHERE id=@Id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", p.Id);
            cmd.Parameters.AddWithValue("@DNI", p.DNI);
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@Contacto", p.Contacto);

            var rows = await cmd.ExecuteNonQueryAsync(ct);
            
            Console.WriteLine($"Updated {rows} rows in personas table for Persona ID {p.Id}");

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"DELETE FROM personas WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = await cmd.ExecuteNonQueryAsync(ct);
            return rows > 0;
        }

        private static Persona MapPersona(MySqlDataReader reader) => new()
        {
            Id       = reader.GetInt32("id"),
            DNI      = reader.GetString("DNI"),
            Nombre   = reader.GetString("nombre"),
            Apellido = reader.GetString("apellido"),
            Contacto = reader.GetString("contacto")
        };
    }
}
