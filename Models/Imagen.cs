using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Imagen
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public String Url { get; set; }
        public IFormFile? Archivo { get; set; }
    }
}