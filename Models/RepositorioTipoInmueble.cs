
using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;

namespace Inmobiliaria_BarrosoEsteban;

    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(TipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO TipoInmueble 
                (Nombre,Estado)
                VALUES (@Nombre,@Estado);
                SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", t.Nombre);
                    command.Parameters.AddWithValue("@Estado", t.Estado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    t.id_tipo = res;
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
                string sql = @"UPDATE TipoInmueble SET Estado = FALSE WHERE id_tipo = @id;";

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
                string sql = @"UPDATE TipoInmueble SET Estado = TRUE WHERE id_tipo = @id;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(TipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE TipoInmueble SET
                Nombre = @Nombre
                WHERE id_tipo = @id;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", t.Nombre);
                    command.Parameters.AddWithValue("@id", t.id_tipo);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }



        public IList<TipoInmueble> Listar()
        {
            var lista = new List<TipoInmueble>();

            using var connection =
                new MySqlConnection(connectionString);

            string sql = @"SELECT
                                id_tipo,
                                Nombre,
                                Estado
                           FROM TipoInmueble
                           ORDER BY nombre;";

            using var command =
                new MySqlCommand(sql, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new TipoInmueble
                {
                    id_tipo = reader.GetInt32("id_tipo"),
                    Nombre = reader.GetString("Nombre"),
                    Estado = reader.GetBoolean("Estado")
                });
            }

            return lista;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipo = null;

            using var connection = new MySqlConnection(connectionString);

            string sql = @"SELECT
                                id_tipo,
                                nombre,
                                estado
                           FROM TipoInmueble
                           WHERE id_tipo = @id;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                tipo = new TipoInmueble
                {
                    id_tipo = reader.GetInt32("id_tipo"),
                    Nombre = reader.GetString("Nombre"),
                    Estado = reader.GetBoolean("Estado")
                };
            }

            return tipo;
        }


    }

