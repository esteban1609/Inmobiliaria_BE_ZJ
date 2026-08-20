using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        List<Inquilino> Listar();
    	Inquilino ObtenerPorId(int id);
    }
}
