using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDSWI.Services
{
    public interface IDaoProducto<T>
    {
        void InsertarProducto(T p);
        void ActualizarProducto(T p);
        void BajaProducto(T p);
        List<T> ListarProductos();
        T BuscarProducto(int id);
    }
}
