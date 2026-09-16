using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioPago
    {
        int Alta(Pago p, int idUsuarioCreador);
        int Anular(int id, int idUsuarioAnulador);
        int ModificarConcepto(int id, string concepto);
        List<Pago> ListarPorReserva(int idReserva,int paginaNro = 1, int tamPagina = 10);
        Pago? ObtenerPorId(int id);
    }
}