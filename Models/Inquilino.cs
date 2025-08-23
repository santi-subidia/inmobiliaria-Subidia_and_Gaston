using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Persona? Persona { get; set; }

        public DateOnly? FechaEliminacion { get; set; }
    }
}
