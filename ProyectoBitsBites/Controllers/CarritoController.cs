using ProyectoBitsBites.Models.TableViewModel;
using ProyectoBitsBites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace ProyectoBitsBites.Controllers
{
    public class CarritoController : Controller
    {
        public static List<ProductoTableViewModel> carrito { get; set; }  = new List<ProductoTableViewModel>() ;

        public ActionResult PedidosList()
        {
            var email = Session["email"];
            if (email == null)
            {
                return RedirectToAction("Login", "Acceso");
            }
            if (Session["rol"].Equals("cliente"))
            {

                Clientes cli = null;
                List<Pedidos> ped = null;
                using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                {
                    cli = (from d in db.Clientes
                           where d.correo_electronico.Equals(email + "")
                           select d).FirstOrDefault();
                    ped = (from d in db.Pedidos
                           where d.id_cliente == cli.id_cliente && d.estado_pedido.Equals("Pendiente")
                           select d).ToList();
                    foreach (var d in ped)
                    {
                        var auxdplist = (from dp in db.DetallesPedido
                                         where dp.id_pedido == d.id_pedido
                                         select dp).ToList();
                        d.DetallesPedido = new List<DetallesPedido>();
                        foreach (var auxdp in auxdplist)
                        {
                            d.DetallesPedido.Add(new DetallesPedido
                            {
                                id_detalle = auxdp.id_detalle,
                                id_pedido = auxdp.id_pedido,
                                id_producto = auxdp.id_producto,
                                cantidad = auxdp.cantidad,
                                subtotal = auxdp.subtotal,
                                Productos = db.Productos.Find(auxdp.id_producto)
                            });
                        };
                    }
                }
                return View(ped);
            } else
            {
                List<Pedidos> ped = null;
                using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                {
                    ped = (from d in db.Pedidos select d).ToList();
                    foreach (var d in ped)
                    {
                        var auxdplist = (from dp in db.DetallesPedido
                                         where dp.id_pedido == d.id_pedido
                                         select dp).ToList();
                        d.Clientes = db.Clientes.Find(d.id_cliente);
                        d.DetallesPedido = new List<DetallesPedido>();
                        foreach (var auxdp in auxdplist)
                        {
                            d.DetallesPedido.Add(new DetallesPedido
                            {
                                id_detalle = auxdp.id_detalle,
                                id_pedido = auxdp.id_pedido,
                                id_producto = auxdp.id_producto,
                                cantidad = auxdp.cantidad,
                                subtotal = auxdp.subtotal,
                                Productos = db.Productos.Find(auxdp.id_producto)
                            });
                        };
                    }
                }
                return View(ped);
            }
        }

        //Se elimina un poducto del carrito
        public ActionResult EliminarProducto(int id)
        {
            for (int i = 0; i < carrito.Count; i++)
            {
                if (carrito[i].ID == id)
                {
                    if (carrito[i].cantidad == 1)
                    {
                        carrito.Remove(carrito[i]);
                    } else
                    {
                        carrito[i].cantidad--;
                    }
                }
            }
            return RedirectToAction("CarritoList", "Carrito");
        }

        public ActionResult SumarProducto(int id)
        {
            // Lógica para agregar el producto al carrito
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
            {
                for (int i = 0; i < carrito.Count; i++)
                {
                    if (carrito[i].ID == id)
                    {
                        carrito[i].cantidad++;
                    }
                }
            }

            return RedirectToAction("CarritoList", "Carrito");
        }

        public ActionResult AgregarProducto(int id)
        {
            // Lógica para agregar el producto al carrito
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
            {
                var can = 1;
                var pos = -1;
                for (int i = 0; i < carrito.Count; i++)
                {
                    if (carrito[i].ID == id)
                    {
                        can = carrito[i].cantidad + 1;
                        pos = i;
                    }
                }
                var producto = db.Productos.Find(id);
                if (producto != null)
                {
                    var productoViewModel = new ProductoTableViewModel
                    {
                        ID = producto.id_producto,
                        Nombre = producto.nombre_producto,
                        Descripcion = producto.descripcion,
                        Precio = producto.precio,
                        Categoria = producto.Categorias.nombre_categoria,
                        id_categoria = producto.id_categoria,
                        Imagen = producto.imagen,
                        cantidad = can
                    };
                    if (pos != -1)
                    {
                        carrito[pos] = productoViewModel;
                    } 
                    else
                    {
                        carrito.Add(productoViewModel);
                    }
                }
            }

            return RedirectToAction("ProductHamburguesas", "Product");
        }

        public ActionResult CarritoList()
        {
            if (Session["User"] != null && Session["rol"].Equals("cliente"))
            {
                return View(carrito);
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult RegistrarPedido()
        {
            var email = Session["email"];
            Clientes cli = null;
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
            {
                cli = (from d in db.Clientes
                             where d.correo_electronico.Equals(email+"")
                             select d).FirstOrDefault();
                Pedidos pedidos = new Pedidos()
                {
                    id_cliente = cli.id_cliente,
                    fecha_pedido = DateTime.Now,
                    estado_pedido = "Pendiente"
                };
                var pedido_re = db.Pedidos.Add(pedidos);
                var total = 0.0;
                for (int i = 0; i < carrito.Count; i++)
                {
                    DetallesPedido detallesPedido = new DetallesPedido()
                    {
                        id_pedido = pedido_re.id_pedido,
                        id_producto = carrito[i].ID,
                        cantidad = carrito[i].cantidad,
                        subtotal = carrito[i].cantidad * carrito[i].Precio
                    };
                    total = total + (double)(carrito[i].Precio * carrito[i].cantidad);
                    db.DetallesPedido.Add(detallesPedido);
                }
                Factura factura = new Factura()
                {
                    id_pedido = pedido_re.id_pedido,
                    total = (decimal?)(total * 1.12),
                    fecha_factura = DateTime.Now
                };
                db.Factura.Add(factura);
                db.SaveChanges();
                carrito = new List<ProductoTableViewModel>();
                return View(factura);
            }
        }

        public ActionResult MostrarCarrito()
        {
            int cantidadProductos = carrito.Count;

            ViewBag.CantidadProductosCarrito = cantidadProductos;
            ViewBag.ImagenCarrito = "https://cdn-icons-png.flaticon.com/512/107/107831.png?w=740&t=st=1689597510~exp=1689598110~hmac=0f0d8907415e059249a0c16e998d643e3960eaeef34f98353d93589532b3356f";

            return PartialView("_CarritoIcono");
        }
    }
}
