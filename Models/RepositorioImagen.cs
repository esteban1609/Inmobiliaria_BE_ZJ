using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;
using System.Data;

namespace Inmobiliaria_BarrosoEsteban.Models
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration)
            : base(configuration)
        {
        }


        // ALTA
        public int Alta(Imagen p)
        {
            int res = -1;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO imagenes
                        (inmueble_id, url)
                    VALUES
                        (@inmuebleId, @url);";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@inmuebleId",
                        p.InmuebleId
                    );

                    command.Parameters.AddWithValue(
                        "@url",
                        p.Url
                    );

                    connection.Open();

                    res = command.ExecuteNonQuery();

                    p.Id = Convert.ToInt32(command.LastInsertedId);
                }
            }

            return res;
        }


        // BAJA
        public int Baja(int id)
        {
            int res = -1;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM imagenes
                    WHERE id_imagen = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }


        // MODIFICACION
        public int Modificacion(Imagen p)
        {
            int res = -1;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE imagenes
                    SET url = @url
                    WHERE id_imagen = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@url", p.Url);

                    connection.Open();

                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }


        // OBTENER POR ID
        public Imagen? ObtenerPorId(int id)
        {
            Imagen? res = null;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT
                        id_imagen,
                        inmueble_id,
                        url
                    FROM imagenes
                    WHERE id_imagen = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        res = new Imagen
                        {
                            Id = reader.GetInt32("id_imagen"),
                            InmuebleId = reader.GetInt32("inmueble_id"),
                            Url = reader.GetString("url")
                        };
                    }
                }
            }

            return res;
        }


        // LISTADO PAGINADO
        public IList<Imagen> ObtenerLista(
            int paginaNro = 1,
            int tamPagina = 10)
        {
            var lista = new List<Imagen>();

            int offset = (paginaNro - 1) * tamPagina;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT
                        id_imagen,
                        inmueble_id,
                        url
                    FROM imagenes
                    ORDER BY id_imagen
                    LIMIT @tamPagina OFFSET @offset;";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@tamPagina",
                        tamPagina
                    );

                    command.Parameters.AddWithValue(
                        "@offset",
                        offset
                    );

                    connection.Open();

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Imagen
                        {
                            Id = reader.GetInt32("id_imagen"),
                            InmuebleId =
                                reader.GetInt32("inmueble_id"),
                            Url = reader.GetString("url")
                        });
                    }
                }
            }

            return lista;
        }


        // CANTIDAD
        public int ObtenerCantidad()
        {
            int res = 0;

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(id_imagen)
                    FROM imagenes;";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    res = Convert.ToInt32(
                        command.ExecuteScalar()
                    );
                }
            }

            return res;
        }


        // BUSCAR POR INMUEBLE
        public IList<Imagen> BuscarPorInmueble(
            int inmuebleId)
        {
            var lista = new List<Imagen>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT
                        id_imagen,
                        inmueble_id,
                        url
                    FROM imagenes
                    WHERE inmueble_id = @inmuebleId;";

                using (MySqlCommand command =
                       new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@inmuebleId",
                        inmuebleId
                    );

                    connection.Open();

                    using var reader =
                        command.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Imagen
                        {
                            Id =
                                reader.GetInt32("id_imagen"),

                            InmuebleId =
                                reader.GetInt32("inmueble_id"),

                            Url =
                                reader.GetString("url")
                        });
                    }
                }
            }

            return lista;
        }
    }
}