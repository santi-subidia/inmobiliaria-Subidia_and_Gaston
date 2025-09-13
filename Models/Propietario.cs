using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    [Table("propietarios")]
    public class Propietario
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // BIGINT -> long

        [Required]
        [MaxLength(20)]
        [Column("dni")]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellido}";

        [MaxLength(50)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(255)]
        [Column("direccion_contacto")]
        public string? DireccionContacto { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("fecha_eliminacion")]
        public DateTime? FechaEliminacion { get; internal set; }
    }
}
