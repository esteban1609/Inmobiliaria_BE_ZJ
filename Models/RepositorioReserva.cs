using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;

namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioReserva : RepositorioBase, IRepositorioReserva
{
    public RepositorioReserva(IConfiguration configuration) : base(configuration)
    {
    }

    // Alta ahora recibe también quién la crea
    public int Alta(Reserva r, int idUsuarioCreador)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO reserva 
            (id_inquilino, id_inmueble, monto_dia, fecha_desde, fecha_hasta, id_usuario_creador)
            VALUES (@idInquilino, @idInmueble, @montoDia, @fechaDesde, @fechaHasta, @idUsuarioCreador);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
                command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
                command.Parameters.AddWithValue("@montoDia", r.MontoDia);
                command.Parameters.AddWithValue("@fechaDesde", r.FechaDesde);
                command.Parameters.AddWithValue("@fechaHasta", r.FechaHasta);
                command.Parameters.AddWithValue("@idUsuarioCreador", idUsuarioCreador);

                connection.Open();
                command.ExecuteNonQuery();

                using (MySqlCommand cmdId = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                {
                    res = Convert.ToInt32(cmdId.ExecuteScalar());
                    r.IdReserva = res;
                }
            }
        }
        return res;
    }

    // Baja ahora registra quién la terminó
    public int Baja(int id, int idUsuarioTerminador)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE reserva SET estado = FALSE, id_usuario_terminador = @idUsuarioTerminador 
            WHERE id_reserva = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idUsuarioTerminador", idUsuarioTerminador);
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
            string sql = @"UPDATE reserva SET estado = TRUE, id_usuario_terminador = NULL 
            WHERE id_reserva = @id;";

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
    public List<Reserva> Listar(int paginaNro = 1, int tamPagina = 10, string? busqueda = null)
    {
        List<Reserva> lista = new List<Reserva>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            // Busca por nombre/apellido del inquilino o dirección del inmueble
            string filtro = string.IsNullOrWhiteSpace(busqueda)
                ? ""
                : "WHERE i.nombre LIKE @busqueda OR i.apellido LIKE @busqueda OR m.direccion LIKE @busqueda";

            string sql = $@"SELECT r.id_reserva, r.id_inquilino, r.id_inmueble, r.monto_dia, 
                            r.fecha_desde, r.fecha_hasta, r.estado,
                            r.id_usuario_creador, r.id_usuario_terminador,r.fecha_terminacion_efectiva,
                            CONCAT(i.nombre, ' ', i.apellido) AS nombre_inquilino,
                            m.direccion AS direccion_inmueble,
                            CONCAT(uc.nombre, ' ', uc.apellido) AS nombre_usuario_creador,
                            CONCAT(ut.nombre, ' ', ut.apellido) AS nombre_usuario_terminador
                            FROM reserva r
                            INNER JOIN inquilino i ON r.id_inquilino = i.id_inquilino
                            INNER JOIN inmueble m ON r.id_inmueble = m.id_inmueble
                            LEFT JOIN usuario uc ON r.id_usuario_creador = uc.id_usuario
                            LEFT JOIN usuario ut ON r.id_usuario_terminador = ut.id_usuario
                            {filtro}
                            ORDER BY r.id_reserva
                            LIMIT @tamPagina OFFSET @offset";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
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



    public IList<Inmueble> MasReservadosUltimos365Dias()
    {
        var lista = new List<Inmueble>();

        using var connection =
            new MySqlConnection(connectionString);

        string sql = @"
        SELECT
            i.id_inmueble,
            i.direccion,
            i.estado,
            p.nombre AS propietario_nombre,
            p.apellido AS propietario_apellido,
            COUNT(r.id_reserva) AS cantidad_reservas

        FROM inmueble i

        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario

        INNER JOIN reserva r
            ON r.id_inmueble = i.id_inmueble

        WHERE r.fecha_desde >= DATE_SUB(CURDATE(), INTERVAL 365 DAY)

        GROUP BY
            i.id_inmueble,
            i.direccion,
            i.estado,
            p.nombre,
            p.apellido

        ORDER BY cantidad_reservas DESC;";

        using var command =
            new MySqlCommand(sql, connection);

        connection.Open();

        using var reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            var inmueble = new Inmueble
            {
                IdInmueble =
                    reader.GetInt32("id_inmueble"),

                Direccion =
                    reader.GetString("direccion"),

                Estado =
                    reader.GetBoolean("estado"),

                CantidadReservas =
                    reader.GetInt32("cantidad_reservas"),

                Propietario = new Propietario
                {
                    Nombre =
                        reader.GetString("propietario_nombre"),

                    Apellido =
                        reader.GetString("propietario_apellido")
                }
            };

            lista.Add(inmueble);
        }

        return lista;
    }


    public IList<Inmueble> SinReservasUltimosDias(int dias)
    {
        var lista = new List<Inmueble>();

        using var connection =
            new MySqlConnection(connectionString);

        string sql = @"
        SELECT
            i.id_inmueble,
            i.direccion,
            i.estado,
            p.nombre AS propietario_nombre,
            p.apellido AS propietario_apellido,
            t.Nombre AS tipo_nombre

        FROM inmueble i

        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario

        INNER JOIN TipoInmueble t
            ON i.id_tipo = t.id_tipo

        WHERE NOT EXISTS
        (
            SELECT 1
            FROM reserva r

            WHERE r.id_inmueble = i.id_inmueble

            AND r.fecha_desde >=
                DATE_SUB(CURDATE(), INTERVAL @dias DAY)
        )

        ORDER BY i.direccion;";

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
            var inmueble = new Inmueble
            {
                IdInmueble =
                    reader.GetInt32("id_inmueble"),

                Direccion =
                    reader.GetString("direccion"),

                Estado =
                    reader.GetBoolean("estado"),

                Propietario = new Propietario
                {
                    Nombre =
                        reader.GetString(
                            "propietario_nombre"),

                    Apellido =
                        reader.GetString(
                            "propietario_apellido")
                },

                TipoInmueble = new TipoInmueble
                {
                    Nombre =
                        reader.GetString(
                            "tipo_nombre")
                }
            };

            lista.Add(inmueble);
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

    public IList<Inmueble> ListarInmueblesDisponibles(
    DateTime fechaDesde,
    DateTime fechaHasta)
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
            i.estado,
            i.id_propietario,
            i.id_tipo,

            p.nombre AS propietario_nombre,
            p.apellido AS propietario_apellido,

            t.Nombre AS tipo_nombre

        FROM inmueble i

        INNER JOIN propietario p
            ON i.id_propietario = p.id_propietario

        INNER JOIN TipoInmueble t
            ON i.id_tipo = t.id_tipo

        WHERE i.estado = TRUE

        AND NOT EXISTS
        (
            SELECT 1
            FROM reserva r

            WHERE r.id_inmueble = i.id_inmueble
              AND r.estado = TRUE

              AND r.fecha_desde <= @fechaHasta
              AND r.fecha_hasta >= @fechaDesde
        )

        ORDER BY i.direccion;";

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@fechaDesde", fechaDesde);

        command.Parameters.AddWithValue("@fechaHasta", fechaHasta);

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

                Estado =
                    reader.GetBoolean("estado"),

                IdPropietario =
                    reader.GetInt32("id_propietario"),

                id_tipo =
                    reader.GetInt32("id_tipo"),

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

    public Reserva? ObtenerPorId(int id)
    {
        Reserva? r = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT r.id_reserva, r.id_inquilino, r.id_inmueble, r.monto_dia, 
                        r.fecha_desde, r.fecha_hasta, r.estado,
                        r.id_usuario_creador, r.id_usuario_terminador,r.fecha_terminacion_efectiva,
                        CONCAT(i.nombre, ' ', i.apellido) AS nombre_inquilino,
                        m.direccion AS direccion_inmueble,
                        CONCAT(uc.nombre, ' ', uc.apellido) AS nombre_usuario_creador,
                        CONCAT(ut.nombre, ' ', ut.apellido) AS nombre_usuario_terminador
                        FROM reserva r
                        INNER JOIN inquilino i ON r.id_inquilino = i.id_inquilino
                        INNER JOIN inmueble m ON r.id_inmueble = m.id_inmueble
                        LEFT JOIN usuario uc ON r.id_usuario_creador = uc.id_usuario
                        LEFT JOIN usuario ut ON r.id_usuario_terminador = ut.id_usuario
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
            DireccionInmueble = reader.GetString(reader.GetOrdinal("direccion_inmueble")),
            IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("id_usuario_creador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
            IdUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("id_usuario_terminador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_terminador")),
            NombreUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("nombre_usuario_creador")) ? null : reader.GetString(reader.GetOrdinal("nombre_usuario_creador")),
            NombreUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("nombre_usuario_terminador")) ? null : reader.GetString(reader.GetOrdinal("nombre_usuario_terminador")),
            FechaTerminacionEfectiva = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_efectiva"))  ? null : reader.GetDateTime(reader.GetOrdinal("fecha_terminacion_efectiva"))
        };
    }

    public bool ExisteSolapamiento(int idInmueble, DateTime fechaDesde, DateTime fechaHasta, int? idReservaExcluir = null)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string filtroExcluir = idReservaExcluir.HasValue ? "AND id_reserva <> @idReservaExcluir" : "";
    
            string sql = $@"SELECT COUNT(*) FROM reserva
                WHERE id_inmueble = @idInmueble
                AND estado = TRUE
                AND fecha_desde <= @fechaHasta
                AND fecha_hasta >= @fechaDesde
                {filtroExcluir};";
    
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
    
                if (idReservaExcluir.HasValue)
                {
                    command.Parameters.AddWithValue("@idReservaExcluir", idReservaExcluir.Value);
                }
    
                connection.Open();
                long cantidad = Convert.ToInt64(command.ExecuteScalar());
                return cantidad > 0;
            }
        }
    }

    public int Terminar(int id, int idUsuarioTerminador, DateTime fechaTerminacionEfectiva)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE reserva SET 
                    estado = FALSE, 
                    id_usuario_terminador = @idUsuarioTerminador,
                    fecha_terminacion_efectiva = @fechaTerminacionEfectiva
                WHERE id_reserva = @id;";
    
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idUsuarioTerminador", idUsuarioTerminador);
                command.Parameters.AddWithValue("@fechaTerminacionEfectiva", fechaTerminacionEfectiva);
                command.Parameters.AddWithValue("@id", id);
    
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }
}