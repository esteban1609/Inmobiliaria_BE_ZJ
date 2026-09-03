using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioReserva
    {
        int Alta(Reserva r);
        int Baja(int id);
        int Modificacion(Reserva r);
        List<Reserva> Listar();
        Reserva? ObtenerPorId(int id);
        int Reactivar(int id);
    }
}