using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDSWI.Services
{
    public interface IDaoCategoria<T>
    {
        List<T> ListarCategorias();
    }
}
