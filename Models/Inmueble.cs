using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria_BarrosoEsteban.Models
{
	[Table("Inmuebles")]
	public class Inmueble
	{
		public int IdInmueble { get; set; }

		[Required(ErrorMessage = "La dirección es obligatoria")]
		public string Direccion { get; set; } = "";

		[Required]
		[Range(1, 100, ErrorMessage = "El cupo debe ser mayor a 0")]
		public int Cupo { get; set; }

		[Required]
		[Range(0.01, double.MaxValue,
			ErrorMessage = "El precio debe ser mayor a 0")]
		public decimal PrecioPorDia { get; set; }

		[Range(0, 100)]
		public decimal PorcentajeReserva { get; set; }

		public int id_tipo {get;set;}

		public TipoInmueble? TipoInmueble { get; set; }

[Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
		public decimal Latitud { get; set; }
		
[Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
		public decimal Longitud { get; set; }

		// FK del propietario
		[Display(Name = "Propietario")]
		public int IdPropietario { get; set; }

		[ForeignKey(nameof(IdPropietario))]
		public Propietario? Propietario { get; set; }

		public bool Estado { get; set; } = true;
	}
}


