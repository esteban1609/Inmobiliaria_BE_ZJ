using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;

namespace Inmobiliaria_BarrosoEsteban;

public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    public RepositorioPago(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Pago p)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO pago (id_reserva, concepto, fecha_pago, importe)
                VALUES (@idReserva, @concepto, @fechaPago, @importe);";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idReserva", p.IdReserva);
                command.Parameters.AddWithValue("@concepto", p.Concepto);
                command.Parameters.AddWithValue("@fechaPago", p.FechaPago);
                command.Parameters.AddWithValue("@importe", p.Importe);

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

    // Baja lógica -> cambia a "anulado", pero el registro sigue existiendo y se sigue listando
    public int Anular(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"UPDATE pago SET estado = FALSE WHERE id_pago = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    // Según la narrativa: al editar un pago, SOLO se puede cambiar el concepto
    // (no el monto ni la fecha), por eso no existe un "Modificacion" genérico como en las otras entidades.
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

    public List<Pago> ListarPorReserva(int idReserva)
    {
        List<Pago> lista = new List<Pago>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id_pago, id_reserva, concepto, fecha_pago, importe, estado
                FROM pago
                WHERE id_reserva = @idReserva
                ORDER BY fecha_pago;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
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
            string sql = @"SELECT id_pago, id_reserva, concepto, fecha_pago, importe, estado
                FROM pago
                WHERE id_pago = @id;";

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
            Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
        };
    }
}