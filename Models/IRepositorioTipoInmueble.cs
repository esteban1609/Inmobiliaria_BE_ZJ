using Inmobiliaria_BarrosoEsteban.Models;


namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioTipoInmueble 
    {
        int Alta(TipoInmueble t);
        int Baja(int id);
        int Modificacion(TipoInmueble t);
        IList<TipoInmueble> Listar(int paginaNro = 1, int tamPagina = 10);
        TipoInmueble? ObtenerPorId(int id);
        int Reactivar(int id);
    }
}