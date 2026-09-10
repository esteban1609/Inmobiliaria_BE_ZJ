using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_BarrosoEsteban.Models;

public class Propietario
{
    [Key]
		[Display(Name = "Código Int.")]
		public int IdPropietario { get; set; }
		[Required(ErrorMessage = "El Nombre  es obligatorio")]
		public string Nombre { get; set; } 
		[Required(ErrorMessage = "El Apellido es obligatorio")]
		public string Apellido { get; set; } 
		[Required(ErrorMessage = "El DNI es obligatorio")]
		public string Dni { get; set; } 
		[Display(Name = "Teléfono")]
		public string Telefono { get; set; } 
		[Required(ErrorMessage = "El mail es obligatorio")]
		[EmailAddress]
		public string Email { get; set; } 
		[Required(ErrorMessage = "La clave es obligatoria"), DataType(DataType.Password)]
		public string Clave { get; set; }
		public Boolean Estado{get; set;} = true;  
}
