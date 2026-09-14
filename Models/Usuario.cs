
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_BarrosoEsteban.Models
{
    public enum Roles
    {
        Administrador=1,
        Empleado=2,
    }

    public class Usuario
    {
        [Key]
        [Display(Name ="codigo")]
        public int id {get; set;}

        [Required]
		public string Nombre { get; set; } = "";

        [Required]
		public string Apellido { get; set; } = "";

        [Required, EmailAddress]
		public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
		public string Clave { get; set; } = "";
    }
}