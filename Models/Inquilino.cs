using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Dni { get; set; }
        [Required]
        public string? Apellido { get; set; }
        [Required]
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Direccion { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? FechaEliminacion { get; set; }
    }
}