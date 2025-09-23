// Inmobiliaria/Repositories/PagoRepository.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;

        public PagoRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // =========================
        // Lecturas
        // =========================
        public async Task<(IEnumerable<Pago> items, int total)> GetPagedAsync(int page, int pageSize, long? contratoId = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var items = new List<Pago>();
            var offset = (page - 1) * pageSize;

            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            // Total
            string sqlCount = "SELECT COUNT(*) FROM pagos";
            if (contratoId.HasValue) sqlCount += " WHERE contrato_id = @contrato_id";

            using var cmdCount = new MySqlCommand(sqlCount, conn);
            if (contratoId.HasValue) cmdCount.Parameters.AddWithValue("@contrato_id", contratoId.Value);

            var total = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

            // Page
            string sql = @"SELECT *
                           FROM pagos";
            if (contratoId.HasValue) sql += " WHERE contrato_id = @contrato_id";
            sql += @" ORDER BY fecha_pago DESC, id DESC
                     LIMIT @limit OFFSET @offset";

            using var cmd = new MySqlCommand(sql, conn);
            if (contratoId.HasValue) cmd.Parameters.AddWithValue("@contrato_id", contratoId.Value);
            cmd.Parameters.AddWithValue("@limit", pageSize);
            cmd.Parameters.AddWithValue("@offset", offset);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                items.Add(MapPago(reader));

            return (items, total);
        }

        public async Task<Pago?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM pagos WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapPago(reader);

            return null;
        }

        // =========================
        // Escrituras
        // =========================
        public async Task<long> CreateAsync(Pago p)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
            INSERT INTO pagos
            (contrato_id, numero_pago, fecha_pago, concepto, importe, estado, creado_por, creado_at, anulado_por, anulado_at)
            VALUES
            (@contrato_id, @numero_pago, @fecha_pago, @concepto, @importe, @estado, @creado_por, @creado_at, @anulado_por, @anulado_at);
            SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contrato_id", p.ContratoId);
            cmd.Parameters.AddWithValue("@numero_pago", p.NumeroPago);
            cmd.Parameters.AddWithValue("@fecha_pago", p.FechaPago);
            cmd.Parameters.AddWithValue("@concepto", p.Concepto);
            cmd.Parameters.AddWithValue("@importe", p.Importe);
            cmd.Parameters.AddWithValue("@estado", p.Estado ?? "Pendiente");
            cmd.Parameters.AddWithValue("@creado_por", p.CreadoPorId ?? 1);
            cmd.Parameters.AddWithValue("@creado_at", p.CreadoAt == default ? DateTime.UtcNow : p.CreadoAt);
            cmd.Parameters.AddWithValue("@anulado_por", (object?)p.AnuladoPorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@anulado_at", (object?)p.AnuladoAt ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Pago p)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
            UPDATE pagos SET
            contrato_id  = @contrato_id,
            numero_pago  = @numero_pago,
            fecha_pago   = @fecha_pago,
            concepto     = @concepto,
            importe      = @importe,
            estado       = @estado
            WHERE id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.Parameters.AddWithValue("@contrato_id", p.ContratoId);
            cmd.Parameters.AddWithValue("@numero_pago", p.NumeroPago);
            cmd.Parameters.AddWithValue("@fecha_pago", p.FechaPago);
            cmd.Parameters.AddWithValue("@concepto", p.Concepto);
            cmd.Parameters.AddWithValue("@importe", p.Importe);
            cmd.Parameters.AddWithValue("@estado", p.Estado ?? "Pendiente");

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        /// <summary>
        /// Soft delete: setea estado='Anulado', anulado_por y anulado_at=UTC.
        /// </summary>
        public async Task<bool> AnularAsync(long id, string anuladoPor)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
            UPDATE pagos SET
            estado      = 'Anulado',
            anulado_por = @anulado_por,
            anulado_at  = @anulado_at
            WHERE id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@anulado_por", anuladoPor);
            cmd.Parameters.AddWithValue("@anulado_at", DateTime.UtcNow);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> ExistsByContratoAndNumeroAsync(long contratoId, int numeroPago, long? excludeId = null)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            var sql = @"SELECT COUNT(*) FROM pagos 
                        WHERE contrato_id=@contrato_id AND numero_pago=@numero_pago";
            if (excludeId.HasValue) sql += " AND id <> @exclude_id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contrato_id", contratoId);
            cmd.Parameters.AddWithValue("@numero_pago", numeroPago);
            if (excludeId.HasValue) cmd.Parameters.AddWithValue("@exclude_id", excludeId.Value);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count > 0;
        }

        public async Task<IEnumerable<Pago>> GetByContratoIdAsync(long contratoId)
        {
            var list = new List<Pago>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                SELECT p.*, c.inquilino_id, c.inmueble_id, c.fecha_inicio, c.fecha_fin_original, 
                       c.fecha_fin_efectiva, c.monto_mensual, c.estado as contrato_estado
                FROM pagos p
                LEFT JOIN contratos c ON p.contrato_id = c.id
                WHERE p.contrato_id = @contrato_id
                ORDER BY p.numero_pago ASC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contrato_id", contratoId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var pago = MapPago(reader);
                if (!reader.IsDBNull(reader.GetOrdinal("inquilino_id")))
                {
                    pago.Contrato = new Contrato
                    {
                        Id = contratoId,
                        InquilinoId = reader.GetInt64("inquilino_id"),
                        InmuebleId = reader.GetInt64("inmueble_id"),
                        FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("fecha_inicio")),
                        FechaFinOriginal = DateOnly.FromDateTime(reader.GetDateTime("fecha_fin_original")),
                        FechaFinEfectiva = reader.IsDBNull(reader.GetOrdinal("fecha_fin_efectiva")) ? null : DateOnly.FromDateTime(reader.GetDateTime("fecha_fin_efectiva")),
                        MontoMensual = reader.GetDecimal("monto_mensual")
                    };
                }
                list.Add(pago);
            }

            return list;
        }

        public async Task<bool> UpdateEstadoAsync(long id, string estado)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                UPDATE pagos 
                SET estado = @estado,
                    anulado_at = @anulado_at,
                    anulado_por = @anulado_por
                WHERE id = @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@anulado_at", estado == "Anulado" ? DateTime.UtcNow : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@anulado_por", estado == "Anulado" ? 1 : (object)DBNull.Value);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<(int cantidadPagos, decimal montoPagado)> GetMontoPagadoAndCantidadPagosByContratoAsync(long contratoId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    COUNT(*) as cantidad_pagos,
                    COALESCE(SUM(importe), 0) as monto_pagado
                FROM pagos 
                WHERE contrato_id = @contrato_id AND estado != 'Anulado'";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contrato_id", contratoId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    cantidadPagos: reader.GetInt32("cantidad_pagos"),
                    montoPagado: reader.GetDecimal("monto_pagado")
                );
            }

            return (0, 0);
        }
        
        public async Task<int> GetCantidadPagosByContratoAsync(long contratoId)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    COUNT(*) as cantidad_pagos
                FROM pagos 
                WHERE contrato_id = @contrato_id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contrato_id", contratoId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("cantidad_pagos");
            }

            return 0;
        }

        // =========================
        // Mapeo
        // =========================
        private static Pago MapPago(MySqlDataReader r) => new()
        {
            Id = r.GetInt32("id"),
            ContratoId = r.GetInt64("contrato_id"),
            NumeroPago = r.GetInt32("numero_pago"),
            FechaPago = r.GetDateOnly("fecha_pago"),
            Concepto = r.GetString("concepto"),
            Importe = r.GetDecimal("importe"),
            Estado = r.GetString("estado"),
            CreadoPorId = r.GetInt32("creado_por"),
            CreadoAt = r.GetDateTime("creado_at"),
            AnuladoPorId = r.IsDBNull(r.GetOrdinal("anulado_por")) ? null : r.GetInt32("anulado_por"),
            AnuladoAt = r.IsDBNull(r.GetOrdinal("anulado_at")) ? null : r.GetDateTime("anulado_at")
        };
    }
}
