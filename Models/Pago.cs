using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_BarrosoEsteban.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "Código Pago")]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "La reserva es obligatoria")]
        [Display(Name = "Reserva")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "El importe es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        // true = activo, false = anulado (baja lógica, pero SIEMPRE se muestra en el listado)
        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        // Solo para mostrar en las vistas
        public string? NombreInquilino { get; set; }
        public string? DireccionInmueble { get; set; }
    }
}