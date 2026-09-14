
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_BarrosoEsteban.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        // Contiene el HASH de la clave, nunca la clave en texto plano
        [Display(Name = "Clave")]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Empleado"; // "Administrador" o "Empleado"

        [Display(Name = "Avatar")]
        public string? Avatar { get; set; }

        public bool Estado { get; set; } = true;
    }
}