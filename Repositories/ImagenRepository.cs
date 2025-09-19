using Inmobiliaria.Models;
using Inmobiliaria.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class ImagenRepository : IImagenRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;

        public ImagenRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Imagen>> GetAllAsync()
        {
            var list = new List<Imagen>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            var cmd = new MySqlCommand("SELECT * FROM imagenes", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapImagen(reader));
            }
            return list;
        }

        public async Task<Imagen?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            var cmd = new MySqlCommand("SELECT * FROM imagenes WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapImagen(reader);
            return null;
        }

        public async Task<long> CreateAsync(Imagen imagen)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            var cmd = new MySqlCommand(@"INSERT INTO imagenes (inmueble_id, url) 
                                       VALUES (@inmueble_id, @url); 
                                       SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@inmueble_id", imagen.InmuebleId);
            cmd.Parameters.AddWithValue("@url", imagen.Url);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Imagen imagen)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            var cmd = new MySqlCommand(@"UPDATE imagenes SET inmueble_id = @inmueble_id, url = @url 
                                       WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", imagen.Id);
            cmd.Parameters.AddWithValue("@inmueble_id", imagen.InmuebleId);
            cmd.Parameters.AddWithValue("@url", imagen.Url);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            var cmd = new MySqlCommand("DELETE FROM imagenes WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<Imagen>> GetByInmuebleIdAsync(int inmuebleId)
        {
            var list = new List<Imagen>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            var cmd = new MySqlCommand("SELECT * FROM imagenes WHERE inmueble_id = @inmueble_id", conn);
            cmd.Parameters.AddWithValue("@inmueble_id", inmuebleId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapImagen(reader));
            }
            return list;
        }

        private static Imagen MapImagen(MySqlDataReader reader) => new()
        {
            Id = reader.GetInt32("id"),
            InmuebleId = reader.GetInt32("inmueble_id"),
            Url = reader.GetString("url")
        };
    }
}