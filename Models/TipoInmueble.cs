

using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_BarrosoEsteban
{
    public class TipoInmueble
    {
        
        [Key]
        public int id_tipo {get; set;}

        [Required]
        public string Nombre {get;set;}

        [Required]

        public Boolean Estado {get;set;}
        

    }
}