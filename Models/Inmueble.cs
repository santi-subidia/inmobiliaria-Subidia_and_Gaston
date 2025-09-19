using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        // FK → propietarios.id
        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name = "Propietario")]
        public int PropietarioId { get; set; }

        // FK → tipos_inmueble.id
        [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
        [Display(Name = "Tipo de Inmueble")]
        public int TipoId { get; set; }

        // Solo comercial o residencial (según tu tabla es un texto)
        [Required(ErrorMessage = "El uso es obligatorio")]
        [Display(Name = "Uso")]
        [StringLength(15, ErrorMessage = "El uso no puede exceder los 15 caracteres")]
        [RegularExpression("^(RESIDENCIAL|COMERCIAL)$",
            ErrorMessage = "El uso debe ser RESIDENCIAL o COMERCIAL")]
        public string? Uso { get; set; }

        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria")]
        [Display(Name = "Ambientes")]
        [Range(1, 50, ErrorMessage = "Ambientes debe estar entre 1 y 50")]
        public int Ambientes { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [Display(Name = "Dirección")]
        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "La dirección debe tener entre 5 y 200 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s#.,-]+$",
            ErrorMessage = "La dirección solo puede contener letras, números, espacios y #.,-")]
        public string? Direccion { get; set; }

        [Display(Name = "Latitud")]
        [Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double? CoordenadaLat { get; set; }

        [Display(Name = "Longitud")]
        [Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double? CoordenadaLon { get; set; }

        [Required(ErrorMessage = "El precio sugerido es obligatorio")]
        [Display(Name = "Precio sugerido (ARS)")]
        [Range(0.01, 1000000000, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioSugerido { get; set; }

        [Display(Name = "Suspendido")]
        public bool Suspendido { get; set; } = false;

        [Display(Name = "Observaciones")]
        [StringLength(255, ErrorMessage = "Las observaciones no pueden exceder los 255 caracteres")]
        public string? Observaciones { get; set; }
        [Display(Name = "Portada")]
        public String? Portada_Url { get; set; }

        public IFormFile? Portada_Archivo { get; set; }

        public List<Imagen>? Imagenes { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Última Actualización")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Propietario? Propietario { get; set; }
        public TipoInmueble? Tipo { get; set; }
    }
}
