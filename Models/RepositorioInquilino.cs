using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;
using System.Data;

namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
{
    public RepositorioInquilino(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Inquilino i)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO inquilino
                (nombre, apellido, dni, telefono, email)
                VALUES (@nombre, @apellido, @dni, @telefono, @email);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", i.Nombre);
                command.Parameters.AddWithValue("@apellido", i.Apellido);
                command.Parameters.AddWithValue("@dni", i.Dni);
                command.Parameters.AddWithValue("@telefono", i.Telefono);
                command.Parameters.AddWithValue("@email", i.Email);

                connection.Open();

                command.ExecuteNonQuery();

                res = Convert.ToInt32(command.LastInsertedId);

                i.IdInquilino = res;
            }
        }

        return res;
    }

    public int Baja(int id)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE FROM inquilino
                           WHERE id_inquilino = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }

        return res;
    }

    public int Modificacion(Inquilino i)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE inquilino SET
                nombre = @nombre,
                apellido = @apellido,
                dni = @dni,
                telefono = @telefono,
                email = @email
                WHERE id_inquilino = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", i.Nombre);
                command.Parameters.AddWithValue("@apellido", i.Apellido);
                command.Parameters.AddWithValue("@dni", i.Dni);
                command.Parameters.AddWithValue("@telefono", i.Telefono);
                command.Parameters.AddWithValue("@email", i.Email);
                command.Parameters.AddWithValue("@id", i.IdInquilino);

                connection.Open();

                res = command.ExecuteNonQuery();
            }
        }

        return res;
    }

    public List<Inquilino> Listar(int paginaNro = 1, int tamPagina = 10, string? busqueda = null)
    {
        List<Inquilino> lista = new List<Inquilino>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string filtro = string.IsNullOrWhiteSpace(busqueda)
                ? ""
                : "WHERE nombre LIKE @busqueda OR apellido LIKE @busqueda OR dni LIKE @busqueda";

            string sql = $@"SELECT id_inquilino, nombre, apellido, dni, telefono, email, estado
                        FROM inquilino
                        {filtro}
                        ORDER BY id_inquilino
                        LIMIT @tamPagina OFFSET @offset";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                int offset = (paginaNro - 1) * tamPagina;

                command.Parameters.AddWithValue("@tamPagina", tamPagina);
                command.Parameters.AddWithValue("@offset", offset);

                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    command.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
                }

                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Inquilino i = new Inquilino
                        {
                            IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                            Dni = reader.GetString(reader.GetOrdinal("dni")),
                            Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
                        };

                        lista.Add(i);
                    }
                }
            }
        }

        return lista;
    }

    public Inquilino? ObtenerPorId(int id)
    {
        Inquilino? i = null;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id_inquilino, nombre, apellido, dni, telefono, email, estado
                           FROM inquilino
                           WHERE id_inquilino = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.CommandType = CommandType.Text;

                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        i = new Inquilino
                        {
                            IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                            Dni = reader.GetString(reader.GetOrdinal("dni")),
                            Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
                        };
                    }
                }
            }
        }

        return i;
    }

    public int Reactivar(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE inquiilino SET estado = TRUE WHERE id_inquilino = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public List<Inquilino> Buscar(string term)
    {
        List<Inquilino> lista = new List<Inquilino>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id_inquilino, dni, nombre, apellido, telefono, email, estado
            FROM inquilino
            WHERE estado = TRUE
              AND (nombre LIKE @term OR apellido LIKE @term OR dni LIKE @term)
            ORDER BY apellido
            LIMIT 20;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@term", $"%{term}%");

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Inquilino
                        {
                            IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                            Dni = reader.GetString(reader.GetOrdinal("dni")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                            Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
                        });
                    }
                }
            }
        }
        return lista;
    }
}