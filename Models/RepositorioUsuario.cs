using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;

namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
{
    public RepositorioUsuario(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Usuario u)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO usuario (nombre, apellido, email, clave, rol, avatar)
                VALUES (@nombre, @apellido, @email, @clave, @rol, @avatar);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", u.Nombre);
                command.Parameters.AddWithValue("@apellido", u.Apellido);
                command.Parameters.AddWithValue("@email", u.Email);
                command.Parameters.AddWithValue("@clave", u.Clave);
                command.Parameters.AddWithValue("@rol", u.Rol);
                command.Parameters.AddWithValue("@avatar", (object?)u.Avatar ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();

                using (MySqlCommand cmdId = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                {
                    res = Convert.ToInt32(cmdId.ExecuteScalar());
                    u.IdUsuario = res;
                }
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE usuario SET estado = FALSE WHERE id_usuario = @id;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public int Reactivar(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE usuario SET estado = TRUE WHERE id_usuario = @id;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    // Edición completa (incluye Rol) -- SOLO la usa el Administrador
    public int Modificacion(Usuario u)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE usuario SET
                nombre = @nombre, apellido = @apellido, email = @email, rol = @rol, avatar = @avatar
                WHERE id_usuario = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", u.Nombre);
                command.Parameters.AddWithValue("@apellido", u.Apellido);
                command.Parameters.AddWithValue("@email", u.Email);
                command.Parameters.AddWithValue("@rol", u.Rol);
                command.Parameters.AddWithValue("@avatar", (object?)u.Avatar ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", u.IdUsuario);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    // Edición de perfil propio (Empleado): NO toca Rol ni Email, solo datos personales/avatar
    public int ModificarPerfilPropio(Usuario u)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE usuario SET
                nombre = @nombre, apellido = @apellido, avatar = @avatar
                WHERE id_usuario = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", u.Nombre);
                command.Parameters.AddWithValue("@apellido", u.Apellido);
                command.Parameters.AddWithValue("@avatar", (object?)u.Avatar ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", u.IdUsuario);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    // Cambiar la clave se hace aparte (ver ActualizarClave en el controlador de cuenta)
    public int ActualizarClave(int id, string nuevoHash)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE usuario SET clave = @clave WHERE id_usuario = @id;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@clave", nuevoHash);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public List<Usuario> Listar()
    {
        List<Usuario> lista = new List<Usuario>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id_usuario, nombre, apellido, email, clave, rol, avatar, estado FROM usuario;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(Mapear(reader));
                    }
                }
            }
        }
        return lista;
    }

    public Usuario? ObtenerPorId(int id)
    {
        Usuario? u = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id_usuario, nombre, apellido, email, clave, rol, avatar, estado 
                FROM usuario WHERE id_usuario = @id;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) u = Mapear(reader);
                }
            }
        }
        return u;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        Usuario? u = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id_usuario, nombre, apellido, email, clave, rol, avatar, estado 
                FROM usuario WHERE email = @email;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) u = Mapear(reader);
                }
            }
        }
        return u;
    }

    private Usuario Mapear(MySqlDataReader reader)
    {
        return new Usuario
        {
            IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Clave = reader.GetString(reader.GetOrdinal("clave")),
            Rol = reader.GetString(reader.GetOrdinal("rol")),
            Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString(reader.GetOrdinal("avatar")),
            Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
        };
    }
}