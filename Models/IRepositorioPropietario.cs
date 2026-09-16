
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		List<Propietario> Listar(int paginaNro = 1, int tamPagina = 10);
    	Propietario ObtenerPorId(int id);
		int Reactivar(int id);
	}
}