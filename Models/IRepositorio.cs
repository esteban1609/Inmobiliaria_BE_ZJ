using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban
{
	public interface IRepositorio<T>
	{
		int Alta(T p);
		int Baja(int id);
		int Modificacion(T p);

		
	}
}