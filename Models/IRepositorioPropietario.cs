
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		List<Propietario> Listar();
    	Propietario ObtenerPorId(int id);
	}
}