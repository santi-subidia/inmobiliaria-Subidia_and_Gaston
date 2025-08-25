using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;
using static Inmobiliaria.Repositories.DateTimeExtensions;

namespace Inmobiliaria.Repositories
{
    public class PropietarioRepository : IPropietarioRepository
    {
        private readonly IMySqlConnectionFactory _factory;
        private readonly PersonaRepository _personaRepo;

        public PropietarioRepository(IMySqlConnectionFactory factory)
        {
            _factory = factory;
            _personaRepo = new PersonaRepository(_factory);
        }

        public async Task<List<Propietario>> GetAllAsync(CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"SELECT pr.id, pr.id_persona, pr.fecha_eliminacion,
                                    p.DNI, p.nombre, p.apellido, p.contacto
                                FROM propietarios pr
                                JOIN personas p ON pr.id_persona = p.id
                                WHERE pr.fecha_eliminacion IS NULL;";

            using var cmd = new MySqlCommand(sql, conn);

            var list = new List<Propietario>();
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                list.Add(MapPropietarioWithPersona(reader));
            }
            return list;
        }

        public async Task<Propietario?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"SELECT p.id, p.id_persona, p.fecha_eliminacion,
                                    per.DNI, per.nombre, per.apellido, per.contacto
                                FROM propietarios p
                                INNER JOIN personas per ON p.id_persona = per.id
                                WHERE p.id=@id AND p.fecha_eliminacion IS NULL
                                LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
                return MapPropietarioWithPersona(reader);

            return null;
        }

        
        public async Task<Propietario?> GetByPersonaIdAsync(int personaId, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"SELECT i.id, i.id_persona, i.fecha_eliminacion,
                                    per.DNI, per.nombre, per.apellido, per.contacto
                                FROM propietarios i
                                INNER JOIN personas per ON i.id_persona = per.id
                                WHERE i.id_persona=@personaId AND i.fecha_eliminacion IS NULL
                                LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@personaId", personaId);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
                return MapPropietarioWithPersona(reader);

            return null;
        }
        public async Task<int> CreateAsync(Propietario p, CancellationToken ct = default)
        {
            String accion = "crear";
            if (p.Persona is null)
                throw new ArgumentNullException(nameof(p.Persona), "La persona no puede ser nula");

            var persona = await _personaRepo.GetByDNIAsync(p.Persona.DNI, ct);
            int personaId;
            if (persona is null)
            {
                personaId = await _personaRepo.CreateAsync(p.Persona, ct);
                if (personaId == 0) return 0;
            }
            else
            {
                var existingPropietario = await GetByPersonaIdAsync(persona.Id, ct);
                if (existingPropietario is not null)
                {
                    accion = "actualizar";
                }
                personaId = persona.Id;
            }

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            String sql;
            if (accion == "actualizar")
            {
                sql = @"UPDATE propietarios SET fecha_eliminacion=@FechaEliminacion WHERE id_persona=@IdPersona";
            }
            else
            {
                sql = @"INSERT INTO propietarios (id_persona, fecha_eliminacion)
                                    VALUES (@IdPersona, @FechaEliminacion);
                                    SELECT LAST_INSERT_ID();";
            }

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdPersona", personaId);
            cmd.Parameters.AddWithValue("@FechaEliminacion", (object)DBNull.Value);

            try
            {
                var result = await cmd.ExecuteScalarAsync(ct);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating inquilino: {ex.Message}");
                return -1;
            }
        }

        public async Task<bool> UpdateAsync(Propietario p, CancellationToken ct = default)
        {
            if (p.Persona is null)
                throw new ArgumentNullException(nameof(p.Persona), "La persona no puede ser nula");

            var rows = await _personaRepo.UpdateAsync(p.Persona, ct);

            return rows;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            const string sql = @"UPDATE propietarios SET fecha_eliminacion=@FechaEliminacion WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@FechaEliminacion", DateTime.Now);

            var rows = await cmd.ExecuteNonQueryAsync(ct);
            return rows > 0;
        }

        private static Propietario MapPropietarioWithPersona(MySqlDataReader reader) => new()
        {
            Id = reader.GetInt32("id"),
            FechaEliminacion = reader.GetOrdinal("fecha_eliminacion") >= 0 && !reader.IsDBNull(reader.GetOrdinal("fecha_eliminacion"))
                ? reader.GetDateTime("fecha_eliminacion").DateOnly()
                : null,
            Persona = new Persona
            {
                Id = reader.GetInt32("id_persona"),
                DNI = reader.GetString("DNI"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Contacto = reader.GetString("contacto")
            }
        };
    }
}

