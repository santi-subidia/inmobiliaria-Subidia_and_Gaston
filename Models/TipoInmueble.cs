using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class TipoInmueble
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 60 caracteres")]
        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Eliminación")]
        public DateTime? FechaEliminacion { get; set; }
    }
}
