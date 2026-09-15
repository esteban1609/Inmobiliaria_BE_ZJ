using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;

namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioReserva : RepositorioBase, IRepositorioReserva
{
    public RepositorioReserva(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Reserva r)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO reserva 
                (id_inquilino, id_inmueble, monto_dia, fecha_desde, fecha_hasta)
                VALUES (@idInquilino, @idInmueble, @montoDia, @fechaDesde, @fechaHasta);
                SELECT LAST_INSERT_ID();";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                command.Parameters.AddWithValue("@montoDia", r.MontoDia);
                command.Parameters.AddWithValue("@fechaDesde", r.FechaDesde);
                command.Parameters.AddWithValue("@fechaHasta", r.FechaHasta);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                r.IdReserva = res;
            }
        }
        return res;
    }

    // Baja lógica
    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE reserva SET estado = FALSE WHERE id_reserva = @id;";

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
            string sql = @"UPDATE reserva SET estado = TRUE WHERE id_reserva = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public int Modificacion(Reserva r)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE reserva SET
                id_inquilino = @idInquilino,
                id_inmueble = @idInmueble,
                monto_dia = @montoDia,
                fecha_desde = @fechaDesde,
                fecha_hasta = @fechaHasta
                WHERE id_reserva = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                command.Parameters.AddWithValue("@montoDia", r.MontoDia);
                command.Parameters.AddWithValue("@fechaDesde", r.FechaDesde);
                command.Parameters.AddWithValue("@fechaHasta", r.FechaHasta);
                command.Parameters.AddWithValue("@id", r.IdReserva);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    // Trae también nombre del inquilino y dirección del inmueble, para que la vista sea legible
    public List<Reserva> Listar()
    {
        List<Reserva> lista = new List<Reserva>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT r.id_reserva, r.id_inquilino, r.id_inmueble, r.monto_dia, 
                    r.fecha_desde, r.fecha_hasta, r.estado,
                    CONCAT(i.nombre, ' ', i.apellido) AS nombre_inquilino,
                    m.direccion AS direccion_inmueble
                FROM reserva r
                INNER JOIN inquilino i ON r.id_inquilino = i.id_inquilino
                INNER JOIN inmueble m ON r.id_inmueble = m.id_inmueble;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearReserva(reader));
                    }
                }
            }
        }
        return lista;
    }


    public IList<Reserva> ListarVigentes()
    {
        var lista = new List<Reserva>();

        using var connection =
            new MySqlConnection(connectionString);

        string sql = @"
        SELECT
            r.id_reserva,
            r.id_inquilino,
            r.id_inmueble,
            r.monto_dia,
            r.fecha_desde,
            r.fecha_hasta,
            r.estado,

            inq.nombre AS inquilino_nombre,
            inq.apellido AS inquilino_apellido,

            i.direccion AS inmueble_direccion

        FROM reserva r

        INNER JOIN inquilino inq
            ON r.id_inquilino = inq.id_inquilino

        INNER JOIN inmueble i
            ON r.id_inmueble = i.id_inmueble

        WHERE r.estado = TRUE
          AND CURDATE() BETWEEN
              DATE(r.fecha_desde)
              AND DATE(r.fecha_hasta)

        ORDER BY r.fecha_hasta ASC;";

        using var command =
            new MySqlCommand(sql, connection);

        connection.Open();

        using var reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            var reserva = new Reserva
            {
                IdReserva =
                    reader.GetInt32("id_reserva"),

                IdInquilino =
                    reader.GetInt32("id_inquilino"),

                IdInmueble =
                    reader.GetInt32("id_inmueble"),

                MontoDia =
                    reader.GetDecimal("monto_dia"),

                FechaDesde =
                    reader.GetDateTime("fecha_desde"),

                FechaHasta =
                    reader.GetDateTime("fecha_hasta"),

                Estado =
                    reader.GetBoolean("estado"),

                Inquilino = new Inquilino
                {
                    Nombre =
                        reader.GetString("inquilino_nombre"),

                    Apellido =
                        reader.GetString("inquilino_apellido")
                },

                Inmueble = new Inmueble
                {
                    Direccion =
                        reader.GetString("inmueble_direccion")
                }
            };

            lista.Add(reserva);
        }

        return lista;
    }

    public IList<Reserva> ListarQueTerminanEnDias(int dias)
    {
        var lista = new List<Reserva>();

        using var connection =
            new MySqlConnection(connectionString);

        string sql = @"
        SELECT
            r.id_reserva,
            r.id_inquilino,
            r.id_inmueble,
            r.monto_dia,
            r.fecha_desde,
            r.fecha_hasta,
            r.estado,

            inq.nombre AS inquilino_nombre,
            inq.apellido AS inquilino_apellido,

            i.direccion AS inmueble_direccion

        FROM reserva r

        INNER JOIN inquilino inq
            ON r.id_inquilino = inq.id_inquilino

        INNER JOIN inmueble i
            ON r.id_inmueble = i.id_inmueble

        WHERE r.estado = TRUE

          AND DATE(r.fecha_hasta)
              BETWEEN CURDATE()
              AND DATE_ADD(
                  CURDATE(),
                  INTERVAL @dias DAY
              )

        ORDER BY r.fecha_hasta ASC;";

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dias",
            dias
        );

        connection.Open();

        using var reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            var reserva = new Reserva
            {
                IdReserva =
                    reader.GetInt32("id_reserva"),

                IdInquilino =
                    reader.GetInt32("id_inquilino"),

                IdInmueble =
                    reader.GetInt32("id_inmueble"),

                MontoDia =
                    reader.GetDecimal("monto_dia"),

                FechaDesde =
                    reader.GetDateTime("fecha_desde"),

                FechaHasta =
                    reader.GetDateTime("fecha_hasta"),

                Estado =
                    reader.GetBoolean("estado"),

                Inquilino = new Inquilino
                {
                    Nombre =
                        reader.GetString("inquilino_nombre"),

                    Apellido =
                        reader.GetString("inquilino_apellido")
                },

                Inmueble = new Inmueble
                {
                    Direccion =
                        reader.GetString("inmueble_direccion")
                }
            };

            lista.Add(reserva);
        }

        return lista;
    }

    public Reserva? ObtenerPorId(int id)
    {
        Reserva? r = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT r.id_reserva, r.id_inquilino, r.id_inmueble, r.monto_dia, 
                    r.fecha_desde, r.fecha_hasta, r.estado,
                    CONCAT(i.nombre, ' ', i.apellido) AS nombre_inquilino,
                    m.direccion AS direccion_inmueble
                FROM reserva r
                INNER JOIN inquilino i ON r.id_inquilino = i.id_inquilino
                INNER JOIN inmueble m ON r.id_inmueble = m.id_inmueble
                WHERE r.id_reserva = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        r = MapearReserva(reader);
                    }
                }
            }
        }
        return r;
    }

    // Método privado para no repetir el mapeo en Listar y ObtenerPorId
    private Reserva MapearReserva(MySqlDataReader reader)
    {
        return new Reserva
        {
            IdReserva = reader.GetInt32(reader.GetOrdinal("id_reserva")),
            IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
            IdInmueble = reader.GetInt32(reader.GetOrdinal("id_inmueble")),
            MontoDia = reader.GetDecimal(reader.GetOrdinal("monto_dia")),
            FechaDesde = reader.GetDateTime(reader.GetOrdinal("fecha_desde")),
            FechaHasta = reader.GetDateTime(reader.GetOrdinal("fecha_hasta")),
            Estado = reader.GetBoolean(reader.GetOrdinal("estado")),
            NombreInquilino = reader.GetString(reader.GetOrdinal("nombre_inquilino")),
            DireccionInmueble = reader.GetString(reader.GetOrdinal("direccion_inmueble"))
        };
    }
}