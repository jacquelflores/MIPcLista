using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoDSWI.Entity
{
    public class Producto1
    {
        public int IdProducto { get; set; }
        public string NombrePro { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Foto { get; set; }
        public int IdCategoria { get; set; }
        public int Stock { get; set; }
    }
}