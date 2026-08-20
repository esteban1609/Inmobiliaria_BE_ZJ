using Inmobiliaria_BarrosoEsteban.Models;
using Npgsql;
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

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"INSERT INTO inquilino
                (nombre, apellido, dni, telefono, email)
                VALUES (@nombre, @apellido, @dni, @telefono, @email)
                RETURNING id_inquilino;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", i.Nombre);
                command.Parameters.AddWithValue("@apellido", i.Apellido);
                command.Parameters.AddWithValue("@dni", i.Dni);
                command.Parameters.AddWithValue("@telefono", i.Telefono);
                command.Parameters.AddWithValue("@email", i.Email);

                connection.Open();

                res = Convert.ToInt32(command.ExecuteScalar());

                i.IdInquilino = res;
                connection.Close();
            }
        }

        return res;
    }

    public int Baja(int id)
    {
        int res = -1;

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"DELETE FROM inquilino
                        WHERE id_inquilino = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        return res;
    }

    public int Modificacion(Inquilino i)
    {
        int res = -1;

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"UPDATE inquilino SET
                nombre = @nombre,
                apellido = @apellido,
                dni = @dni,
                telefono = @telefono,
                email = @email
                WHERE id_inquilino = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", i.Nombre);
                command.Parameters.AddWithValue("@apellido", i.Apellido);
                command.Parameters.AddWithValue("@dni", i.Dni);
                command.Parameters.AddWithValue("@telefono", i.Telefono);
                command.Parameters.AddWithValue("@email", i.Email);
                command.Parameters.AddWithValue("@id", i.IdInquilino);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        return res;
    }

public List<Inquilino> Listar()
{
    List<Inquilino> lista = new List<Inquilino>();

    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
    {
        string sql = @"SELECT id_inquilino, nombre, apellido, dni, telefono, email
                       FROM inquilino;";

        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.CommandType = CommandType.Text;

            connection.Open();

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Inquilino i = new Inquilino
                {
                    IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                    Dni = reader.GetString(reader.GetOrdinal("dni")),
                    Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                    Email = reader.GetString(reader.GetOrdinal("email"))
                };

                lista.Add(i);
            }

            connection.Close();
        }
    }

    return lista;
}

public Inquilino? ObtenerPorId(int id)
{
    Inquilino? i = null;

    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
    {
        string sql = @"SELECT id_inquilino, nombre, apellido, dni, telefono, email
                       FROM inquilino
                       WHERE id_inquilino = @id;";

        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            command.CommandType = CommandType.Text;

            connection.Open();

            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                i = new Inquilino
                {
                    IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                    Dni = reader.GetString(reader.GetOrdinal("dni")),
                    Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                    Email = reader.GetString(reader.GetOrdinal("email"))
                };
            }

            connection.Close();
        }
    }

    return i;
}
}