
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_BarrosoEsteban
{
    
    public interface IRepositiorio<T>
    {
        int alta(T p);
        int baja(int id);
        int modificacion(T p);
    }

}