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
                	(Direccion, Cupo, Precio_Por_Dia, Porcentaje_Reserva, Latitud, Longitud, id_propietario, id_tipo)
					VALUES (@direccion, @cupo, @precioPorDia, @porcentajeReserva, @latitud, @longitud, @propietarioId, @tipo);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@direccion", i.Direccion == null ? DBNull.Value : i.Direccion);
                command.Parameters.AddWithValue("@cupo", i.Cupo);
                command.Parameters.AddWithValue("@precioPorDia", i.PrecioPorDia);
                command.Parameters.AddWithValue("@porcentajeReserva", i.PorcentajeReserva);
                command.Parameters.AddWithValue("@latitud", i.Latitud);
                command.Parameters.AddWithValue("@longitud", i.Longitud);
                command.Parameters.AddWithValue("@propietarioId", i.IdPropietario);
                command.Parameters.AddWithValue("@tipo", i.id_tipo);
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
                        direccion = @direccion,
                        cupo = @cupo,
                        precio_por_dia = @precioPorDia,
                        porcentaje_reserva = @porcentajeReserva,
                        latitud = @latitud,
                        longitud = @longitud,
                        id_propietario = @propietarioId,
                        id_tipo = @tipoId
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
                command.Parameters.AddWithValue("@tipoId", i.id_tipo);
                command.Parameters.AddWithValue("@id", i.IdInmueble);

                connection.Open();
                res = command.ExecuteNonQuery();
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
            i.id_tipo,
            i.portada,
            p.nombre AS propietario_nombre,
            p.apellido AS propietario_apellido,
            t.Nombre AS tipo_nombre
        FROM inmueble i

        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario

        INNER JOIN TipoInmueble t
            ON i.id_tipo = t.id_tipo

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
                id_tipo = reader.GetInt32("id_tipo"),
                Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Nombre = reader.GetString("propietario_nombre"),
                    Apellido = reader.GetString("propietario_apellido")
                },
                TipoInmueble = new TipoInmueble
                {
                    id_tipo = reader.GetInt32("id_tipo"),
                    Nombre = reader.GetString("tipo_nombre")
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
                    i.id_tipo,
                    i.portada,
                    p.nombre AS propietario_nombre,
                    p.apellido AS propietario_apellido,
                    t.Nombre AS tipo_nombre
                FROM inmueble i

                INNER JOIN propietario p
                    ON i.id_propietario = p.id_propietario
                    
                INNER JOIN TipoInmueble t
                    ON i.id_tipo = t.id_tipo

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
                id_tipo = reader.GetInt32("id_tipo"),
                Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32("id_propietario"),
                    Nombre = reader.GetString("propietario_nombre"),
                    Apellido = reader.GetString("propietario_apellido")
                },
                TipoInmueble = new TipoInmueble
                {
                    id_tipo = reader.GetInt32("id_tipo"),
                    Nombre = reader.GetString("tipo_nombre")
                }
            };

            lista.Add(inmueble);
        }

        return lista;
    }




    public IList<Inmueble> ListarPorEstado(bool estado)
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
            i.id_tipo,
            i.portada,
            p.nombre AS propietario_nombre,
            p.apellido AS propietario_apellido,
            t.Nombre AS tipo_nombre
        FROM inmueble i

        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario

        INNER JOIN TipoInmueble t
            ON i.id_tipo = t.id_tipo

        WHERE i.estado = @estado

        ORDER BY i.id_inmueble;";

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@estado",
            estado
        );

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var inmueble = new Inmueble
            {
                IdInmueble =
                    reader.GetInt32("id_inmueble"),

                Direccion =
                    reader.GetString("direccion"),

                Cupo =
                    reader.GetInt32("cupo"),

                PrecioPorDia =
                    reader.GetDecimal("precio_por_dia"),

                PorcentajeReserva =
                    reader.GetDecimal("porcentaje_reserva"),

                Latitud =
                    reader.GetDecimal("latitud"),

                Longitud =
                    reader.GetDecimal("longitud"),

                IdPropietario =
                    reader.GetInt32("id_propietario"),

                Estado =
                    reader.GetBoolean("estado"),

                id_tipo =
                    reader.GetInt32("id_tipo"),

                Portada =
                    reader.IsDBNull(
                        reader.GetOrdinal("portada"))
                    ? null
                    : reader.GetString("portada"),

                Propietario = new Propietario
                {
                    IdPropietario =
                        reader.GetInt32("id_propietario"),

                    Nombre =
                        reader.GetString(
                            "propietario_nombre"),

                    Apellido =
                        reader.GetString(
                            "propietario_apellido")
                },

                TipoInmueble = new TipoInmueble
                {
                    id_tipo =
                        reader.GetInt32("id_tipo"),

                    Nombre =
                        reader.GetString("tipo_nombre")
                }
            };

            lista.Add(inmueble);
        }

        return lista;
    }


    public IList<Inmueble> ListarPorPropietario(int idPropietario)
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
            i.id_tipo,
            i.portada,
            p.nombre AS propietario_nombre,
            p.apellido AS propietario_apellido,
            t.Nombre AS tipo_nombre
        FROM inmueble i

        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario

        INNER JOIN TipoInmueble t
            ON i.id_tipo = t.id_tipo

        WHERE i.id_propietario = @idPropietario

        ORDER BY i.id_inmueble;";

    using var command =
        new MySqlCommand(sql, connection);

    command.Parameters.AddWithValue(
        "@idPropietario",
        idPropietario
    );

    connection.Open();

    using var reader = command.ExecuteReader();

    while (reader.Read())
    {
        var inmueble = new Inmueble
        {
            IdInmueble =
                reader.GetInt32("id_inmueble"),

            Direccion =
                reader.GetString("direccion"),

            Cupo =
                reader.GetInt32("cupo"),

            PrecioPorDia =
                reader.GetDecimal("precio_por_dia"),

            PorcentajeReserva =
                reader.GetDecimal("porcentaje_reserva"),

            Latitud =
                reader.GetDecimal("latitud"),

            Longitud =
                reader.GetDecimal("longitud"),

            IdPropietario =
                reader.GetInt32("id_propietario"),

            Estado =
                reader.GetBoolean("estado"),

            id_tipo =
                reader.GetInt32("id_tipo"),

            Portada =
                reader.IsDBNull(
                    reader.GetOrdinal("portada"))
                ? null
                : reader.GetString("portada"),

            Propietario = new Propietario
            {
                IdPropietario =
                    reader.GetInt32("id_propietario"),

                Nombre =
                    reader.GetString("propietario_nombre"),

                Apellido =
                    reader.GetString("propietario_apellido")
            },

            TipoInmueble = new TipoInmueble
            {
                id_tipo =
                    reader.GetInt32("id_tipo"),

                Nombre =
                    reader.GetString("tipo_nombre")
            }
        };

        lista.Add(inmueble);
    }

    return lista;
}

    public int ModificarPortada(int id, string url)
    {
        int res = -1;

        using (MySqlConnection connection =
               new MySqlConnection(connectionString))
        {
            string sql = @"
            UPDATE inmueble
            SET portada = @portada
            WHERE id_inmueble = @id;";

            using (MySqlCommand command =
                   new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@portada", url);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();

                res = command.ExecuteNonQuery();
            }
        }

        return res;
    }

}
