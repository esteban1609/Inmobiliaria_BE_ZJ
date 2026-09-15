using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria_BarrosoEsteban.Models
{
    [Table("Reserva")]
    public class Reserva
    {
        [Key]
        [Display(Name = "Código Reserva")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "El inquilino es obligatorio")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [ForeignKey(nameof(IdInquilino))]
        public Inquilino? Inquilino { get; set; }

        [Required(ErrorMessage = "El inmueble es obligatorio")]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        [ForeignKey(nameof(IdInmueble))]
        public Inmueble? Inmueble { get; set; }

        [Required(ErrorMessage = "El monto por día es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto diario debe ser mayor a 0")]
        [Display(Name = "Monto por Día")]
        [DataType(DataType.Currency)]
        public decimal MontoDia { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Desde")]
        public DateTime FechaDesde { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Hasta")]
        public DateTime FechaHasta { get; set; } = DateTime.Today.AddDays(1);

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        // Propiedades de navegación/lectura para las vistas HTML (no mapeadas directamente a la BD)
        [Display(Name = "Nombre Inquilino")]
        public string? NombreInquilino { get; set; }

        [Display(Name = "Dirección Inmueble")]
        public string? DireccionInmueble { get; set; }

        // Auditoría -- solo se muestra a Administrador
        public int? IdUsuarioCreador { get; set; }
        public int? IdUsuarioTerminador { get; set; }
        public string? NombreUsuarioCreador { get; set; }
        public string? NombreUsuarioTerminador { get; set; }
    }
}

