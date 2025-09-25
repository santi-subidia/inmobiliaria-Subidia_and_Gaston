using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key, Column("id")]
        public long Id { get; set; }

        [Required, StringLength(255)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [StringLength(500)]
        [Url]
        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Required]
        [Column("rol_id")]
        public int RolId { get; set; } // FK a rol_usuarios(id)

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Required, Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required, Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
