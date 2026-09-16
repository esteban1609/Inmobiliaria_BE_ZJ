using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        List<Inquilino> Listar();
    	Inquilino? ObtenerPorId(int id);
        int Reactivar(int id);
        List<Inquilino> Buscar(string term);
    }
}
