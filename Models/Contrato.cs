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

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public EstadoContrato Estado { get; set; } = EstadoContrato.VIGENTE;

        [Display(Name = "Renovado de ID")]
        public long? RenovadoDeId { get; set; }

        [Required(ErrorMessage = "El creador es obligatorio")]
        [Display(Name = "Creado por")]
        public long CreadoPor { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.DateTime)]
        public DateTime CreadoAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Finalizado por")]
        public long? FinalizadoPor { get; set; }

        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.DateTime)]
        public DateTime? FinalizadoAt { get; set; }

        public Inquilino? Inquilino { get; set; }
        //public Inmueble? Inmueble { get; set; }
        //public Usuario? Creador { get; set; }
        //public Usuario? Finalizador { get; set; }

        // Métodos de utilidad
        public bool EstaVigente => Estado == EstadoContrato.VIGENTE &&
                                  FechaInicio <= DateOnly.FromDateTime(DateTime.Now) &&
                                  (FechaFinEfectiva ?? FechaFinOriginal) >= DateOnly.FromDateTime(DateTime.Now);

        public int DuracionEnMeses => 
            ((FechaFinOriginal.Year - FechaInicio.Year) * 12) + 
            (FechaFinOriginal.Month - FechaInicio.Month);

        public decimal MontoTotal => MontoMensual * DuracionEnMeses;

        public bool PuedeFinalizarse => Estado == EstadoContrato.VIGENTE;

        public bool PuedeRescindirse => Estado == EstadoContrato.VIGENTE;
    }

    public enum EstadoContrato
    {
        [Display(Name = "Vigente")]
        VIGENTE,
        
        [Display(Name = "Finalizado")]
        FINALIZADO,
        
        [Display(Name = "Rescindido")]
        RESCINDIDO
    }
}