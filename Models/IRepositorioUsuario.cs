using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
    public interface IRepositorioUsuario
    {
        int Alta(Usuario u);
        int Baja(int id);
        int Reactivar(int id);
        int Modificacion(Usuario u);         // usado por Administrador para editar a cualquier usuario
        int ModificarPerfilPropio(Usuario u); // usado por Empleado: solo sus propios datos, sin tocar Rol
        int ActualizarClave(int id, string nuevoHash);
        List<Usuario> Listar(int paginaNro = 1, int tamPagina = 10, string? busqueda = null);

        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email); // clave para el login
    }
}