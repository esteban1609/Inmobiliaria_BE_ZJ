using Inmobiliaria_BarrosoEsteban.Models;
using MySqlConnector;
using System.Data;

namespace Inmobiliaria_BarrosoEsteban.Models
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration) : base(configuration)
        {

        }


        public int Alta(Imagen p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Imagenes 
					(InmuebleId, Url) 
					VALUES (@inmuebleId, @url)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inmuebleId", p.InmuebleId);
                    command.Parameters.AddWithValue("@url", p.Url);
                    connection.Open();
                    res = command.ExecuteNonQuery();
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
                string sql = @$"DELETE FROM Imagenes WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Imagen p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"
				UPDATE Imagenes SET 
					Url=@url
				WHERE Id=@id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@url", p.Url);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }


        public Imagen? ObtenerPorId(int id)
        {
            Imagen? res = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes
					WHERE {nameof(Imagen.Id)}=@id";
                using (MySqlCommand comm = new MySqlCommand(sql, connection))
                {
                    comm.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Imagen();
                        res.Id = reader.GetInt32(nameof(Imagen.Id));
                        res.InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId));
                        res.Url = reader.GetString(nameof(Imagen.Url));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Imagen> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			List<Imagen> res = new List<Imagen>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes
					ORDER BY Id
					OFFSET {(paginaNro - 1) * tamPagina} ROW
					FETCH NEXT {tamPagina} ROWS ONLY
				";
				using (MySqlCommand comm = new MySqlCommand(sql, connection))
				{
					connection.Open();
					var reader = comm.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Imagen
						{
							Id = reader.GetInt32(nameof(Imagen.Id)),
							InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId)),
							Url = reader.GetString(nameof(Imagen.Url)),
						});
					}
					connection.Close();
				}
			}
			return res;
		}

        public int ObtenerCantidad()
		{
			int res = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT COUNT(Id)
					FROM Imagenes
				";
				using (MySqlCommand comm = new MySqlCommand(sql, connection))
				{
					comm.CommandType = CommandType.Text;
					connection.Open();
					res = Convert.ToInt32(comm.ExecuteScalar());
					connection.Close();
				}
			}
			return res;
		}

		public IList<Imagen> BuscarPorInmueble(int inmuebleId)
		{
			List<Imagen> res = new List<Imagen>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes
					WHERE {nameof(Imagen.InmuebleId)}=@inmuebleId";
				using (MySqlCommand comm = new MySqlCommand(sql, connection))
				{
					comm.Parameters.AddWithValue("@inmuebleId", inmuebleId);
					connection.Open();
					var reader = comm.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Imagen
						{
							Id = reader.GetInt32(nameof(Imagen.Id)),
							InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId)),
							Url = reader.GetString(nameof(Imagen.Url)),
						});
					}
					connection.Close();
				}
			}
			return res;
		}
	

    }
}