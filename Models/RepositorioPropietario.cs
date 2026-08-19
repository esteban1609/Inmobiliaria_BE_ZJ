using Inmobiliaria_BarrosoEsteban.Models;
using Npgsql;
using System.Data;

namespace Inmobiliaria_BarrosoEsteban;



public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
{
    public RepositorioPropietario(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Propietario p)
    {
        int res = -1;
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"INSERT INTO propietario 
                (nombre, apellido, dni, telefono, email)
                VALUES (@nombre, @apellido, @dni, @telefono, @email)
                RETURNING id_propietario;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                p.IdPropietario = res;
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"DELETE FROM propietario WHERE id_propietario = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                res = command.ExecuteNonQuery(); // devuelve la cantidad de filas afectadas
            }
        }
        return res;
    }

    public int Modificacion(Propietario p)
    {
        int res = -1;
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"UPDATE propietario SET
                nombre = @nombre,
                apellido = @apellido,
                dni = @dni,
                telefono = @telefono,
                email = @email
                WHERE id_propietario = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@id", p.IdPropietario);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }
}