using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoBitsBites.Models.TableViewModel
{
    public class ProductoTableViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public byte[] Imagen { get; set; }
        public Nullable<int> id_categoria { get; set; }
        public int cantidad { get; set; }
        public string Categoria { get; set; }
    }
}