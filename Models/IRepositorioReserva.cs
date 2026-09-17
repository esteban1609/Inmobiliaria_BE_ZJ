using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioReserva
    {
        int Alta(Reserva r, int idUsuarioCreador);
        int Baja(int id, int idUsuarioTerminador);
        int Reactivar(int id);
        int Modificacion(Reserva r);
        List<Reserva> Listar(int paginaNro = 1, int tamPagina = 10, string? busqueda = null);
        IList<Reserva> ListarVigentes();
        IList<Inmueble> MasReservadosUltimos365Dias();

        IList<Inmueble> SinReservasUltimosDias(int dias);
        IList<Reserva> ListarQueTerminanEnDias(int dias);

        IList<Inmueble> ListarInmueblesDisponibles(DateTime fechaDesde,DateTime fechaHasta);


        Reserva? ObtenerPorId(int id);
        bool ExisteSolapamiento(int idInmueble, DateTime fechaDesde, DateTime fechaHasta, int? idReservaExcluir = null);

        int Terminar(int id, int idUsuarioTerminador, DateTime fechaTerminacionEfectiva);
        
    }
}