using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        [Key]
        [Display(Name = "ID")]
        public long Id { get; set; }

        [Required(ErrorMessage = "El inmueble es obligatorio")]
        [Display(Name = "ID Inmueble")]
        public long InmuebleId { get; set; }

        [Required(ErrorMessage = "El inquilino es obligatorio")]
        [Display(Name = "ID Inquilino")]
        public long InquilinoId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateOnly FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin original es obligatoria")]
        [Display(Name = "Fecha de Fin Original")]
        [DataType(DataType.Date)]
        public DateOnly FechaFinOriginal { get; set; }

        [Display(Name = "Fecha de Fin Efectiva")]
        [DataType(DataType.Date)]
        public DateOnly? FechaFinEfectiva { get; set; }

        [Required(ErrorMessage = "El monto mensual es obligatorio")]
        [Display(Name = "Monto Mensual")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto mensual debe ser mayor a 0")]
        public decimal MontoMensual { get; set; }

        [Display(Name = "Estado")]
        public String Estado()
        {
            if (FechaFinEfectiva.HasValue && FechaFinEfectiva < FechaFinOriginal)
            {
                return "RESCINDIDO";
            }
            else if (FechaFinOriginal < DateOnly.FromDateTime(DateTime.Now))
            {
                return "FINALIZADO";
            }
            else
            {
                return "VIGENTE";
            }
        }

        //[Required(ErrorMessage = "El creador es obligatorio")]
        [Display(Name = "Creado por")]
        public long? CreadoPor { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.DateTime)]
        public DateTime CreadoAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Finalizado por")]
        public long? FinalizadoPor { get; set; }

        [Display(Name = "Fecha de Eliminación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaEliminacion { get; set; }

        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }
        //public Usuario? Creador { get; set; }
        //public Usuario? Finalizador { get; set; }

        public int DuracionEnMeses (DateOnly fechaPost) =>
            ((fechaPost.Year - FechaInicio.Year) * 12) +
            (fechaPost.Month - FechaInicio.Month);

        public decimal MontoTotal => MontoMensual * DuracionEnMeses(FechaFinOriginal);

        public decimal MontoMulta => FechaFinEfectiva.HasValue && FechaFinEfectiva < FechaFinOriginal
            ? MontoMensual * multipliciadMulta()
            : 0;

        public bool PuedeFinalizarse => Estado() == "VIGENTE" && FechaEliminacion == null;

        public bool PuedeRescindirse => Estado() == "VIGENTE" && FechaEliminacion == null;

        private int multipliciadMulta()
        {
            if (FechaFinEfectiva.HasValue && DuracionEnMeses(FechaFinOriginal) / 2 <= DuracionEnMeses(FechaFinEfectiva.Value))
            {
                return 1;
            }
            else
            {
                return 2;

            }
        }
    }
}