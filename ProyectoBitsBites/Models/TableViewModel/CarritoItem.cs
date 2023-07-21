using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoBitsBites.Models.TableViewModel
{
    public class CarritoItem
    {
        //Tabla Pedidos
        public int ID_Pedido { get; set; }
        public Nullable<int> ID_Cliente { get; set; }
        public Nullable<System.DateTime> Fecha_Pedido { get; set; }
        public string Estado_Pedido { get; set; }

        //Tabla DetallesPedido
        public int ID_Detalle { get; set; }
        public Nullable<int> ID_Producto { get; set; }
        public Nullable<int> Cantidad { get; set; }
        public Nullable<decimal> Precio { get; set; }

    }
}