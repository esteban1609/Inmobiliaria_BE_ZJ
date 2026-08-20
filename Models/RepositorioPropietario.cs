using Inmobiliaria_BarrosoEsteban.Models;
using Npgsql;

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
                (nombre, apellido, dni, telefono, email, clave)
                VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave)
                RETURNING IdPropietario;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@clave", p.Clave);

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
            string sql = @"DELETE FROM propietario WHERE IdPropietario = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                res = command.ExecuteNonQuery();
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
                email = @email,
                clave = @clave
                WHERE IdPropietario = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@clave", p.Clave);
                command.Parameters.AddWithValue("@id", p.IdPropietario);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public List<Propietario> Listar()
    {
        List<Propietario> lista = new List<Propietario>();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"SELECT IdPropietario, nombre, apellido, dni, telefono, email, clave 
                FROM propietario;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            IdPropietario = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                            Dni = reader.GetString(reader.GetOrdinal("dni")),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                            Clave = reader.GetString(reader.GetOrdinal("clave"))
                        };
                        lista.Add(p);
                    }
                }
            }
        }
        return lista;
    }

    public Propietario ObtenerPorId(int id)
    {
        Propietario p = null;
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"SELECT IdPropietario, nombre, apellido, dni, telefono, email, clave 
                FROM propietario 
                WHERE idpropietario = @id;";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        p = new Propietario
                        {
                            IdPropietario = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                            Dni = reader.GetString(reader.GetOrdinal("dni")),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                            Clave = reader.GetString(reader.GetOrdinal("clave"))
                        };
                    }
                }
            }
        }
        return p;
    }
}