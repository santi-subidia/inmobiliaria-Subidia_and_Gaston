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

        private const string VigenteSQL = "CURDATE() < c.fecha_fin_original AND c.fecha_fin_efectiva IS NULL";
        private const string FinalizadoSQL = "CURDATE() > c.fecha_fin_original AND c.fecha_fin_efectiva IS NULL";
        private const string RescindidoSQL = "c.fecha_fin_efectiva IS NOT NULL AND c.fecha_fin_efectiva < c.fecha_fin_original";
        private const string NoEliminadoSQL = "c.fecha_eliminacion IS NULL";

        public ContratoRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Contrato>> GetAllAsync()
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE {NoEliminadoSQL}
                ORDER BY c.creado_at DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<(IEnumerable<Contrato> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? tipoEstado = null, bool noEliminado = true)
        {
            var list = new List<Contrato>();

            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            // Construir WHERE dinámicamente
            var whereClause = "";

            if (!string.IsNullOrEmpty(tipoEstado)) {
                if (tipoEstado.ToUpper() == "VIGENTE") {
                    whereClause = $"AND {VigenteSQL}";
                } else if (tipoEstado.ToUpper() == "FINALIZADO") {
                    whereClause = $"AND {FinalizadoSQL}";
                } else if (tipoEstado.ToUpper() == "RESCINDIDO") {
                    whereClause = $"AND {RescindidoSQL}";
                }
            }

            if (noEliminado) {
                whereClause += $" AND {NoEliminadoSQL}";
            }

            string sql = $@"
                SELECT SQL_CALC_FOUND_ROWS c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE 1=1
                {whereClause}
                ORDER BY c.creado_at DESC
                LIMIT @pageSize OFFSET @offset;
                SELECT FOUND_ROWS();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);
            cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using var reader = await cmd.ExecuteReaderAsync();

            // --- 1° resultado: la página de datos ---
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }

            // --- 2° resultado: total de registros ---
            await reader.NextResultAsync();
            int total = 0;
            if (await reader.ReadAsync())
            {
                total = reader.GetInt32(0);
            }

            return (list, total);
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

        public async Task<IEnumerable<Contrato>> GetVigentesEnRangoAsync(DateOnly fechaDesde, DateOnly fechaHasta)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE {VigenteSQL}
                AND c.fecha_inicio <= @fechaHasta
                AND c.fecha_fin_original >= @fechaDesde
                ORDER BY c.fecha_inicio ASC";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fechaDesde", fechaDesde);
            cmd.Parameters.AddWithValue("@fechaHasta", fechaHasta);
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

        public async Task<IEnumerable<Contrato>> GetVigentesAsync()
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE {VigenteSQL} AND {NoEliminadoSQL}
                ORDER BY c.fecha_inicio DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Contrato>> GetFinalizadosAsync()
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE {FinalizadoSQL} AND {NoEliminadoSQL}
                ORDER BY c.fecha_inicio DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Contrato>> GetRescindidosAsync()
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE {RescindidoSQL} AND {NoEliminadoSQL}
                ORDER BY c.fecha_inicio DESC";
                
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Contrato>> GetProximosAVencerAsync(int dias = 30)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE {VigenteSQL} AND {NoEliminadoSQL}
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
            
            string sql = $@"
                SELECT c.*, i.dni, i.apellido, i.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos i ON c.inquilino_id = i.id
                WHERE c.inmueble_id = @inmuebleId AND {VigenteSQL} AND {NoEliminadoSQL}
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
                INSERT INTO contratos (inmueble_id, inquilino_id, fecha_inicio, fecha_fin_original, monto_mensual, creado_por, creado_at)
                VALUES (@inmueble_id, @inquilino_id, @fecha_inicio, @fecha_fin_original, @monto_mensual, @creado_por, @creado_at);
                SELECT LAST_INSERT_ID();";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmueble_id", contrato.InmuebleId);
            cmd.Parameters.AddWithValue("@inquilino_id", contrato.InquilinoId);
            cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_original", contrato.FechaFinOriginal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
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
                    fecha_fin_efectiva = @fecha_fin_efectiva, monto_mensual = @monto_mensual
                WHERE id = @id";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", contrato.Id);
            cmd.Parameters.AddWithValue("@inmueble_id", contrato.InmuebleId);
            cmd.Parameters.AddWithValue("@inquilino_id", contrato.InquilinoId);
            cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_original", contrato.FechaFinOriginal.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", contrato.FechaFinEfectiva?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
            
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long idContrato, long idUsuario)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = @"
                UPDATE contratos 
                SET fecha_eliminacion = @fecha_eliminacion, eliminado_por = @eliminado_por
                WHERE id = @id AND fecha_eliminacion IS NULL";

            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idContrato);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@eliminado_por", idUsuario);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> FinalizarContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            // Solo verificamos que esté vigente usando la lógica de fechas y actualizamos las fechas
            string sql = $@"
                UPDATE contratos c
                SET c.finalizado_por = @finalizado_por,
                    c.fecha_fin_efectiva = COALESCE(@fecha_fin_efectiva, c.fecha_fin_efectiva, CURDATE())
                WHERE c.id = @id AND {VigenteSQL} AND {NoEliminadoSQL}";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@finalizado_por", finalizadoPor);
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", fechaFinEfectiva?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd"));
            
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> RescindirContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            // Para rescisión, establecemos fecha_fin_efectiva anterior a fecha_fin_original
            string sql = $@"
                UPDATE contratos c
                SET c.finalizado_por = @finalizado_por,
                    c.fecha_fin_efectiva = @fecha_fin_efectiva
                WHERE c.id = @id AND {VigenteSQL} AND {NoEliminadoSQL}";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@finalizado_por", finalizadoPor);
            // Para rescisión usamos la fecha proporcionada o hoy si es anterior a la original
            cmd.Parameters.AddWithValue("@fecha_fin_efectiva", fechaFinEfectiva?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd"));
            
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> ExisteContratoVigenteParaInmuebleAsync(long inmuebleId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT COUNT(*) FROM contratos c
                WHERE c.inmueble_id = @inmuebleId AND {VigenteSQL} AND {NoEliminadoSQL}";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
            
            var count = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        public async Task<bool> ExisteContratoVigenteParaInquilinoAsync(long inquilinoId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT COUNT(*) FROM contratos c
                WHERE c.inquilino_id = @inquilinoId AND {VigenteSQL} AND {NoEliminadoSQL}";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inquilinoId", inquilinoId);
            
            var count = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }

        public async Task<(IEnumerable<Contrato> Items, int TotalCount)> GetPagedWithFiltersAsync(int page, int pageSize, string? tipoEstado = null, long? propietarioId = null, long? inmuebleId = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null)
        {
            var list = new List<Contrato>();

            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            // Construir WHERE dinámicamente
            var whereConditions = new List<string>();
            var parameters = new List<MySqlParameter>();

            // Siempre excluimos eliminados
            whereConditions.Add(NoEliminadoSQL);

            if (!string.IsNullOrEmpty(tipoEstado))
            {
                if (tipoEstado.ToUpper() == "VIGENTE")
                {
                    whereConditions.Add(VigenteSQL);
                }
                else if (tipoEstado.ToUpper() == "FINALIZADO")
                {
                    whereConditions.Add(FinalizadoSQL);
                }
                else if (tipoEstado.ToUpper() == "RESCINDIDO")
                {
                    whereConditions.Add(RescindidoSQL);
                }
            }

            if (propietarioId.HasValue)
            {
                whereConditions.Add("i.propietario_id = @propietarioId");
                parameters.Add(new MySqlParameter("@propietarioId", propietarioId.Value));
            }

            if (inmuebleId.HasValue)
            {
                whereConditions.Add("c.inmueble_id = @inmuebleId");
                parameters.Add(new MySqlParameter("@inmuebleId", inmuebleId.Value));
            }

            if (fechaDesde.HasValue)
            {
                whereConditions.Add("c.fecha_inicio <= @fechaDesde");
                parameters.Add(new MySqlParameter("@fechaDesde", fechaDesde.Value.ToString("yyyy-MM-dd")));
            }

            if (fechaHasta.HasValue)
            {
                whereConditions.Add("c.fecha_fin_original >= @fechaHasta");
                parameters.Add(new MySqlParameter("@fechaHasta", fechaHasta.Value.ToString("yyyy-MM-dd")));
            }

            var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

            // SQL con paginado y filtros
            string sql = $@"
                SELECT SQL_CALC_FOUND_ROWS c.*, 
                       inq.dni, inq.apellido, inq.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos inq ON c.inquilino_id = inq.id
                LEFT JOIN inmuebles i ON c.inmueble_id = i.id
                {whereClause}
                ORDER BY c.creado_at DESC
                LIMIT @offset, @pageSize";

            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);
            cmd.Parameters.AddRange(parameters.ToArray());

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }

            // Obtener el total
            await reader.CloseAsync();
            var countCmd = new MySqlCommand("SELECT FOUND_ROWS()", conn);
            var total = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

            return (list, total);
        }

        public async Task<IEnumerable<Contrato>> GetProximosVencerAsync(int dias)
        {
            var list = new List<Contrato>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            
            string sql = $@"
                SELECT c.*, inq.dni, inq.apellido, inq.nombre 
                FROM contratos c 
                LEFT JOIN inquilinos inq ON c.inquilino_id = inq.id
                WHERE {VigenteSQL} AND {NoEliminadoSQL}
                AND c.fecha_fin_original BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)
                ORDER BY c.fecha_fin_original ASC";
                
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dias", dias);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapContrato(reader));
            }
            return list;
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
                CreadoPor = reader.IsDBNull(reader.GetOrdinal("creado_por")) ? null : reader.GetInt64("creado_por"),
                CreadoAt = reader.GetDateTime("creado_at"),
                FinalizadoPor = reader.IsDBNull(reader.GetOrdinal("finalizado_por")) ? null : reader.GetInt64("finalizado_por"),
                FechaEliminacion = reader.IsDBNull(reader.GetOrdinal("fecha_eliminacion")) ? null : reader.GetDateTime("fecha_eliminacion")
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