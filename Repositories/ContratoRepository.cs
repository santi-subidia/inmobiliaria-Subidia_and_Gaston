using Inmobiliaria.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using Inmobiliaria.Data;
using Inmobiliaria.Repositories;

namespace Inmobiliaria.Repositories
{
    public class ContratoRepository : IContratoRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;

        public ContratoRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Contrato>> GetAllAsync()
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                ORDER BY c.creado_at DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<Contrato?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.id = @id";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapContrato(reader);
            return null;
        }

        public async Task<IEnumerable<Contrato>> GetByInquilinoIdAsync(long inquilinoId)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.inquilino_id = @inquilinoId
                ORDER BY c.fecha_inicio DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inquilinoId", inquilinoId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Contrato>> GetByInmuebleIdAsync(long inmuebleId)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.inmueble_id = @inmuebleId
                ORDER BY c.fecha_inicio DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Contrato>> GetByEstadoAsync(EstadoContrato estado)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.estado = @estado
                ORDER BY c.fecha_inicio DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@estado", estado.ToString());
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Contrato>> GetVigentesAsync()
        {
            return await GetByEstadoAsync(EstadoContrato.VIGENTE);
        }

        public async Task<IEnumerable<Contrato>> GetProximosAVencerAsync(int dias = 30)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.estado = 'VIGENTE' 
                AND COALESCE(c.fecha_fin_efectiva, c.fecha_fin_original) <= DATE_ADD(CURDATE(), INTERVAL @dias DAY)
                AND COALESCE(c.fecha_fin_efectiva, c.fecha_fin_original) >= CURDATE()
                ORDER BY COALESCE(c.fecha_fin_efectiva, c.fecha_fin_original) ASC";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dias", dias);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<Contrato?> GetContratoVigenteByInmuebleAsync(long inmuebleId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.inmueble_id = @inmuebleId AND c.estado = 'VIGENTE'
                LIMIT 1";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapContrato(reader);
            return null;
        }

        public async Task<long> CreateAsync(Contrato contrato)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                INSERT INTO contratos (inmueble_id, inquilino_id, fecha_inicio, fecha_fin_original, 
                    fecha_fin_efectiva, monto_mensual, estado, renovado_de_id, creado_por, creado_at)
                VALUES (@inmueble_id, @inquilino_id, @fecha_inicio, @fecha_fin_original, 
                    @fecha_fin_efectiva, @monto_mensual, @estado, @renovado_de_id, @creado_por, @creado_at);
                SELECT LAST_INSERT_ID();";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmueble_id", contrato.InmuebleId);
            cmd.Parameters.AddWithValue("@inquilino_id", contrato.InquilinoId);
            cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_original", contrato.FechaFinOriginal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", contrato.FechaFinEfectiva?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
            cmd.Parameters.AddWithValue("@estado", contrato.Estado.ToString());
            cmd.Parameters.AddWithValue("@renovado_de_id", contrato.RenovadoDeId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@creado_por", contrato.CreadoPor);
            cmd.Parameters.AddWithValue("@creado_at", DateTime.UtcNow);
            
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Contrato contrato)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                UPDATE contratos 
                SET inmueble_id = @inmueble_id, inquilino_id = @inquilino_id, 
                    fecha_inicio = @fecha_inicio, fecha_fin_original = @fecha_fin_original,
                    fecha_fin_efectiva = @fecha_fin_efectiva, monto_mensual = @monto_mensual,
                    estado = @estado, renovado_de_id = @renovado_de_id
                WHERE id = @id";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", contrato.Id);
            cmd.Parameters.AddWithValue("@inmueble_id", contrato.InmuebleId);
            cmd.Parameters.AddWithValue("@inquilino_id", contrato.InquilinoId);
            cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_original", contrato.FechaFinOriginal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", contrato.FechaFinEfectiva?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
            cmd.Parameters.AddWithValue("@estado", contrato.Estado.ToString());
            cmd.Parameters.AddWithValue("@renovado_de_id", contrato.RenovadoDeId ?? (object)DBNull.Value);
            
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            // En este caso, eliminar un contrato podría ser peligroso
            // Se recomienda cambiar el estado a RESCINDIDO en su lugar
            return await RescindirContratoAsync(id, 1); // Asumiendo usuario sistema = 1
        }

        public async Task<bool> FinalizarContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                UPDATE contratos 
                SET estado = 'FINALIZADO', 
                    finalizado_por = @finalizado_por, 
                    finalizado_at = @finalizado_at,
                    fecha_fin_efectiva = COALESCE(@fecha_fin_efectiva, fecha_fin_efectiva, CURDATE())
                WHERE id = @id AND estado = 'VIGENTE'";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@finalizado_por", finalizadoPor);
            cmd.Parameters.AddWithValue("@finalizado_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", fechaFinEfectiva?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> RescindirContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                UPDATE contratos 
                SET estado = 'RESCINDIDO', 
                    finalizado_por = @finalizado_por, 
                    finalizado_at = @finalizado_at,
                    fecha_fin_efectiva = COALESCE(@fecha_fin_efectiva, CURDATE())
                WHERE id = @id AND estado = 'VIGENTE'";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@finalizado_por", finalizadoPor);
            cmd.Parameters.AddWithValue("@finalizado_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", fechaFinEfectiva?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> ExisteContratoVigenteParaInmuebleAsync(long inmuebleId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT COUNT(*) FROM contratos 
                WHERE inmueble_id = @inmuebleId AND estado = 'VIGENTE'";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
            
            var count = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        public async Task<bool> ExisteContratoVigenteParaInquilinoAsync(long inquilinoId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                SELECT COUNT(*) FROM contratos 
                WHERE inquilino_id = @inquilinoId AND estado = 'VIGENTE'";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inquilinoId", inquilinoId);
            
            var count = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        private static Contrato MapContrato(MySqlDataReader reader)
        {
            var contrato = new Contrato
            {
                Id = reader.GetInt64("id"),
                InmuebleId = reader.GetInt64("inmueble_id"),
                InquilinoId = reader.GetInt64("inquilino_id"),
                FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("fecha_inicio")),
                FechaFinOriginal = DateOnly.FromDateTime(reader.GetDateTime("fecha_fin_original")),
                FechaFinEfectiva = reader.IsDBNull(reader.GetOrdinal("fecha_fin_efectiva")) ? null : DateOnly.FromDateTime(reader.GetDateTime("fecha_fin_efectiva")),
                MontoMensual = reader.GetDecimal("monto_mensual"),
                Estado = Enum.Parse<EstadoContrato>(reader.GetString("estado")),
                RenovadoDeId = reader.IsDBNull(reader.GetOrdinal("renovado_de_id")) ? null : reader.GetUInt32("renovado_de_id"),
                CreadoPor = reader.GetInt64("creado_por"),
                CreadoAt = reader.GetDateTime("creado_at"),
                FinalizadoPor = reader.IsDBNull(reader.GetOrdinal("finalizado_por")) ? null : reader.GetInt64("finalizado_por"),
                FinalizadoAt = reader.IsDBNull(reader.GetOrdinal("finalizado_at")) ? null : reader.GetDateTime("finalizado_at")
            };
                if (!reader.IsDBNull(reader.GetOrdinal("inquilino_id")))
                {
                    contrato.Inquilino = new Inquilino
                    {
                        Id = reader.GetInt32("inquilino_id"),
                        Dni = reader.IsDBNull(reader.GetOrdinal("dni")) ? null : reader.GetString("dni"),
                        Apellido = reader.IsDBNull(reader.GetOrdinal("apellido")) ? null : reader.GetString("apellido"),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString("nombre")
                    };
                }
            return contrato;
        }
    }
}