using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios")]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "El teléfono no tiene un formato válido")]
        public string? Telefono { get; set; }

        [StringLength(500)]
        [Url(ErrorMessage = "La URL del avatar no es válida")]
        public string? AvatarUrl { get; set; }

        [Required]
        public int RolId { get; set; } // FK a rol_usuarios(id)

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
