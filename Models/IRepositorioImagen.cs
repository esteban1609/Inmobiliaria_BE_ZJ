
namespace Inmobiliaria_BarrosoEsteban.Models
{
    	public interface IRepositorioImagen : IRepositorio<Imagen>
	{
		IList<Imagen> BuscarPorInmueble(int inmuebleId);
        Imagen? ObtenerPorId(int id);
	}
}