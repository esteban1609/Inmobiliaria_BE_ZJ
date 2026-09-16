using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;

namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    public RepositorioPago(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Pago p, int idUsuarioCreador)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO pago (id_reserva, concepto, fecha_pago, importe, id_usuario_creador)
                VALUES (@idReserva, @concepto, @fechaPago, @importe, @idUsuarioCreador);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idReserva", p.IdReserva);
                command.Parameters.AddWithValue("@concepto", p.Concepto);
                command.Parameters.AddWithValue("@fechaPago", p.FechaPago);
                command.Parameters.AddWithValue("@importe", p.Importe);
                command.Parameters.AddWithValue("@idUsuarioCreador", idUsuarioCreador);

                connection.Open();
                command.ExecuteNonQuery();

                using (MySqlCommand cmdId = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                {
                    res = Convert.ToInt32(cmdId.ExecuteScalar());
                    p.IdPago = res;
                }
            }
        }
        return res;
    }

    public int Anular(int id, int idUsuarioAnulador)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE pago SET estado = FALSE, id_usuario_anulador = @idUsuarioAnulador 
                WHERE id_pago = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idUsuarioAnulador", idUsuarioAnulador);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public int ModificarConcepto(int id, string concepto)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE pago SET concepto = @concepto WHERE id_pago = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@concepto", concepto);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public List<Pago> ListarPorReserva(int idReserva, int paginaNro = 1, int tamPagina = 10)
    {
        List<Pago> lista = new List<Pago>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT p.id_pago, p.id_reserva, p.concepto, p.fecha_pago, p.importe, p.estado,
                    p.id_usuario_creador, p.id_usuario_anulador,
                    CONCAT(uc.nombre, ' ', uc.apellido) AS nombre_usuario_creador,
                    CONCAT(ua.nombre, ' ', ua.apellido) AS nombre_usuario_anulador
                FROM pago p
                LEFT JOIN usuario uc ON p.id_usuario_creador = uc.id_usuario
                LEFT JOIN usuario ua ON p.id_usuario_anulador = ua.id_usuario
                WHERE p.id_reserva = @idReserva
                ORDER BY p.fecha_pago
                LIMIT @tamPagina OFFSET @offset";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {

                int offset = (paginaNro - 1) * tamPagina;

                command.Parameters.AddWithValue("@tamPagina", tamPagina);
                command.Parameters.AddWithValue("@offset", offset);
                command.Parameters.AddWithValue("@idReserva", idReserva);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearPago(reader));
                    }
                }
            }
        }
        return lista;
    }

    public Pago? ObtenerPorId(int id)
    {
        Pago? p = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT p.id_pago, p.id_reserva, p.concepto, p.fecha_pago, p.importe, p.estado,
                    p.id_usuario_creador, p.id_usuario_anulador,
                    CONCAT(uc.nombre, ' ', uc.apellido) AS nombre_usuario_creador,
                    CONCAT(ua.nombre, ' ', ua.apellido) AS nombre_usuario_anulador
                FROM pago p
                LEFT JOIN usuario uc ON p.id_usuario_creador = uc.id_usuario
                LEFT JOIN usuario ua ON p.id_usuario_anulador = ua.id_usuario
                WHERE p.id_pago = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        p = MapearPago(reader);
                    }
                }
            }
        }
        return p;
    }

    private Pago MapearPago(MySqlDataReader reader)
    {
        return new Pago
        {
            IdPago = reader.GetInt32(reader.GetOrdinal("id_pago")),
            IdReserva = reader.GetInt32(reader.GetOrdinal("id_reserva")),
            Concepto = reader.GetString(reader.GetOrdinal("concepto")),
            FechaPago = reader.GetDateTime(reader.GetOrdinal("fecha_pago")),
            Importe = reader.GetDecimal(reader.GetOrdinal("importe")),
            Estado = reader.GetBoolean(reader.GetOrdinal("estado")),
            IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("id_usuario_creador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
            IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("id_usuario_anulador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_anulador")),
            NombreUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("nombre_usuario_creador")) ? null : reader.GetString(reader.GetOrdinal("nombre_usuario_creador")),
            NombreUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("nombre_usuario_anulador")) ? null : reader.GetString(reader.GetOrdinal("nombre_usuario_anulador"))
        };
    }
}