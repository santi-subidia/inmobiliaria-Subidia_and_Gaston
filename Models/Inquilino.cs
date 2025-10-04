using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [Display(Name = "DNI", Prompt = "Sin puntos ni comas", Description = "Documento Nacional de Identidad")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "El DNI debe tener entre 7 y 10 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El DNI solo puede contener números")]
        public string? Dni { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string? Nombre { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", 
            ErrorMessage = "Formato de teléfono inválido. Ej: 2664-123456")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
            ErrorMessage = "Formato de email inválido. Debe ser: usuario@dominio.com")]
        public string? Email { get; set; }

        [Display(Name = "Fecha de Eliminación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaEliminacion { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public static Inquilino InquilinoFromPropietario(Propietario propietario)
        {
            return new Inquilino
            {
                Dni = propietario.Dni,
                Apellido = propietario.Apellido,
                Nombre = propietario.Nombre,
                Telefono = propietario.Telefono,
                Email = propietario.Email,
            };
        }
    }
}