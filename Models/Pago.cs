using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        // FK → contratos.id
        [Required(ErrorMessage = "El contrato es obligatorio")]
        [Display(Name = "Contrato")]
        public long ContratoId { get; set; }

        // (Opcional) navegación si ya tenés el modelo Contrato
        public Contrato? Contrato { get; set; }

        [Required(ErrorMessage = "El número de pago es obligatorio")]
        [Display(Name = "Nº de Pago")]
        [Range(1, 120, ErrorMessage = "El número de pago debe estar entre 1 y 120")]
        public int NumeroPago { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.Date)]
        public DateOnly FechaPago { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [Display(Name = "Concepto")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El concepto debe tener entre 3 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s#.,-]+$",
            ErrorMessage = "El concepto solo puede contener letras, números, espacios y los caracteres #.,-")]
        public string? Concepto { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio")]
        [Display(Name = "Importe")]
        [DataType(DataType.Currency)]
        [Range(0.01, 999999999.99, ErrorMessage = "El importe debe ser mayor a 0")]
        [Column("importe", TypeName = "decimal(18,2)")]
        public decimal Importe { get; set; }

        // Guardado como texto para seguir tu estilo (sin enum).
        // Si preferís enum, te paso la variante.
        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        [StringLength(20)]
        [RegularExpression("^(Pendiente|Pagado|Anulado)$",
            ErrorMessage = "Estado inválido. Valores permitidos: Pagado, Anulado")]
        public string? Estado { get; set; } = "Pagado";

        // Auditoría
        [Required]
        [Display(Name = "Creado por")]
        public int CreadoPorId { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.DateTime)]
        public DateTime CreadoAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Anulado por")]
        public int? AnuladoPorId { get; set; }

        [Display(Name = "Fecha de Anulación")]
        [DataType(DataType.DateTime)]
        public DateTime? AnuladoAt { get; set; }

        [Display(Name = "¿Está Anulado?")]
        public bool EstaAnulado => (Estado?.Equals("Anulado", StringComparison.OrdinalIgnoreCase) ?? false) || AnuladoAt.HasValue;

        public string ImporteFormateado => Importe.ToString("C");

        public Contrato? contrato { get; set; }
        public Usuario? creadoPor { get; set; }
        public Usuario? anuladoPor { get; set; }
    }
}
