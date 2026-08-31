using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;
using System.Data;
namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
{
    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {

    }



    public int Alta(Inmueble i)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO inmueble
                	(Direccion, Cupo, PrecioPorDia, PorcentajeReserva, Latitud, Longitud, PropietarioId)
					VALUES (@direccion, @cupo, @precioPorDia, @porcentajeReserva, @latitud, @longitud, @propietarioId);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@direccion", i.Direccion == null ? DBNull.Value : i.Direccion);
                command.Parameters.AddWithValue("@cupo", i.Cupo);
                command.Parameters.AddWithValue("@precioPorDia", i.PrecioPorDia);
                command.Parameters.AddWithValue("@porcentajeReserva", i.PorcentajeReserva);
                command.Parameters.AddWithValue("@latitud", i.Latitud);
                command.Parameters.AddWithValue("@longitud", i.Longitud);
                command.Parameters.AddWithValue("@propietarioId", i.IdPropietario);
                connection.Open();
                command.ExecuteNonQuery();
                res = Convert.ToInt32(command.LastInsertedId);
                i.IdInmueble = res;
                connection.Close();
            }
        }

        return res;
    }

    public int Baja(int id)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
        string sql = @"UPDATE inmueble
                       SET estado = FALSE
                       WHERE id_inmueble = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        return res;
    }


    public int Reactivar(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE inmueble SET estado = TRUE WHERE id_inmueble = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public int Modificacion(Inmueble i)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE inmueble SET
						Direccion=@direccion, Cupo=@cupo, PrecioPorDia=@precioPorDia, PorcentajeReserva=@porcentajeReserva, 
						Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId
                WHERE id_inmueble = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@direccion", i.Direccion);
                command.Parameters.AddWithValue("@cupo", i.Cupo);
                command.Parameters.AddWithValue("@precioPorDia", i.PrecioPorDia);
                command.Parameters.AddWithValue("@porcentajeReserva", i.PorcentajeReserva);
                command.Parameters.AddWithValue("@latitud", i.Latitud);
                command.Parameters.AddWithValue("@longitud", i.Longitud);
                command.Parameters.AddWithValue("@propietarioId", i.IdPropietario);
                command.Parameters.AddWithValue("@id", i.IdInmueble);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        return res;
    }



    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? inmueble = null;

        using var connection = new MySqlConnection(connectionString);

        string sql = @"
        SELECT
            i.id_inmueble,
            i.direccion,
            i.cupo,
            i.precio_por_dia,
            i.porcentaje_reserva,
            i.latitud,
            i.longitud,
            i.id_propietario,
            i.estado,
            p.nombre,
            p.apellido
        FROM inmueble i
        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario
        WHERE i.id_inmueble = @id";

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", id);

        connection.Open();

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            inmueble = new Inmueble
            {
                IdInmueble = reader.GetInt32("id_inmueble"),
                Direccion = reader.GetString("direccion"),
                Cupo = reader.GetInt32("cupo"),
                PrecioPorDia = reader.GetDecimal("precio_por_dia"),
                PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                Latitud = reader.GetDecimal("latitud"),
                Longitud = reader.GetDecimal("longitud"),
                IdPropietario = reader.GetInt32("id_propietario"),
                Estado = reader.GetBoolean("estado"),
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido")
                }
            };
        }

        return inmueble;
    }


    public IList<Inmueble> Listar()
    {
        var lista = new List<Inmueble>();

        using var connection =
            new MySqlConnection(connectionString);

        string sql = @"
                SELECT
                    i.id_inmueble,
                    i.direccion,
                    i.cupo,
                    i.precio_por_dia,
                    i.porcentaje_reserva,
                    i.latitud,
                    i.longitud,
                    i.id_propietario,
                    i.estado,
                    p.nombre,
                    p.apellido
                FROM inmueble i
                INNER JOIN propietario p
                    ON i.id_propietario = p.id_propietario
                ORDER BY i.id_inmueble";

        using var command = new MySqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var inmueble = new Inmueble
            {
                IdInmueble = reader.GetInt32("id_inmueble"),
                Direccion = reader.GetString("direccion"),
                Cupo = reader.GetInt32("cupo"),
                PrecioPorDia = reader.GetDecimal("precio_por_dia"),
                PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                Latitud = reader.GetDecimal("latitud"),
                Longitud = reader.GetDecimal("longitud"),
                IdPropietario = reader.GetInt32("id_propietario"),
                Estado = reader.GetBoolean("estado"),
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido")
                }
            };

            lista.Add(inmueble);
        }

        return lista;
    }

}
