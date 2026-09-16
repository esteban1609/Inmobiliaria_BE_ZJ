namespace Inmobiliaria_BarrosoEsteban.Models
{
    public interface IRepositorioInmueble
    {
        IList<Inmueble> Listar();
        IList<Inmueble> ListarPorEstado(bool estado);

        IList<Inmueble> ListarPorPropietario(int idPropietario);

        int Alta(Inmueble inmueble);

        Inmueble? ObtenerPorId(int id);

        int ModificarPortada(int InmuebleId, string ruta);
        int Modificacion(Inmueble inmueble);

        int Baja(int id);

        int Reactivar(int id);
        List<Inmueble> Buscar(string term);
    }
}