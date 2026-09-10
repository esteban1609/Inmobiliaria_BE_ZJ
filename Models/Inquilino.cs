
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_BarrosoEsteban.Models;

	public class Inquilino
	{
		[Key]
		[Display(Name = "Código")]
		public int IdInquilino { get; set; }
		[Required(ErrorMessage = "El nombre es obligatorio")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "El apellido es obligatorio")]
		public string Apellido { get; set; }
		[Required(ErrorMessage = "El DNI es obligatorio")]
		public string Dni { get; set; }
		public string Telefono { get; set; }
		[Required(ErrorMessage = "El mail es obligatorio")]
		[EmailAddress]
		public string Email { get; set; }

		public Boolean Estado{get;set;}=true;
	}


