using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Persona
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 o 8 dígitos seguidos")]
        public string DNI { get; set; } = "";

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El contacto es obligatorio")]
        [StringLength(100, ErrorMessage = "El contacto no puede superar los 100 caracteres")]
        [EmailAddress(ErrorMessage = "El contacto debe ser un correo electrónico válido")]
        public string Contacto { get; set; } = "";
    }
}
