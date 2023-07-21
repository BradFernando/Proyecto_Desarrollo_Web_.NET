using ProyectoBitsBites.Models;
using ProyectoBitsBites.Models.TableViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoBitsBites.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product cliente
        //Cliente
        //Esta funcion lista todos los productos por su categoria y los muestra en la vista ProductHamburguesas
        public ActionResult ProductHamburguesas(int id_categoria = 0)
        {
            // Aqui se debe validar que el usuario sea cliente y no administrador
            if (Session["User"] != null && Session["rol"].Equals("cliente"))
            {
                // Llama al metodo para listar los productos de la categoria hamburguesas
                List<ProductoTableViewModel> lst = null;
                // Si el id_categoria es cero, se listan todos los productos de la base de datos
                if (id_categoria == 0)
                {
                    using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                    {
                        lst = (from d in db.Productos
                               where d.estado.Equals("1")
                               orderby d.nombre_producto
                               select new ProductoTableViewModel
                               {
                                   ID = d.id_producto,
                                   Nombre = d.nombre_producto,
                                   Descripcion = d.descripcion,
                                   Precio = d.precio,
                                   Categoria = d.Categorias.nombre_categoria,
                                   Imagen = d.imagen
                               }).ToList();
                    }
                }
                else
                {
                    // Si no es cero, se filtra por la categoria seleccionada en el menu de navegacion
                    using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                    {
                        lst = (from d in db.Productos
                               where d.id_categoria == id_categoria && d.estado.Equals("1")
                               orderby d.nombre_producto
                               select new ProductoTableViewModel
                               {
                               
                                   ID = d.id_producto,
                                   Nombre = d.nombre_producto,
                                   Descripcion = d.descripcion,
                                   Precio = d.precio,
                                   Categoria = d.Categorias.nombre_categoria,
                                   id_categoria = d.id_categoria,
                                   Imagen = d.imagen
                               }).ToList();
                    }
                }
                return View(lst);
            }
            // Si no es cliente, se redirecciona al login con un mensaje de error
            return RedirectToAction("Login", "Acceso");
        }
        //Administrador
        // Esta los lista para el caso del administrador aqui llamamos todos los productos 
        public ActionResult ProductList()
        {
        
            // Aqui se debe validar que el usuario sea administrador y no cliente
            if (Session["User"] != null && Session["rol"].Equals("administrador"))
            {
                List<ProductoTableViewModel> lst = null;
                using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                    {
                        lst = (from d in db.Productos
                               where d.estado.Equals("1")
                               orderby d.nombre_producto
                               select new ProductoTableViewModel
                               {
                                   ID = d.id_producto,
                                   Nombre = d.nombre_producto,
                                   Descripcion = d.descripcion,
                                   Precio = d.precio,
                                   Categoria = d.Categorias.nombre_categoria,
                                   id_categoria = d.id_categoria,
                                   Imagen = d.imagen
                               }).ToList();
                    }
                return View(lst);
            }
            // Si no es administrador, se redirecciona al login con un mensaje de error 
            return RedirectToAction("Login", "Acceso", new { Msg = "Debe ingresar con una cuenta de adminitrador" });
        }
        //Admin
        // Esta funcion retorna la vista para crear un nuevo producto o editar uno existente
        public ActionResult ProductForm(int id = 0)
        {
            // Si el id es cero, se esta creando un nuevo producto y se retorna la vista vacia
            ProductoTableViewModel producto = new ProductoTableViewModel();
            if (id != 0)
            {
                using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                {
                    var aux = db.Productos.Find(id);
                    producto.ID = aux.id_producto;
                    producto.Nombre = aux.nombre_producto;
                    producto.Descripcion = aux.descripcion;
                    producto.Categoria = aux.Categorias.nombre_categoria;
                    producto.id_categoria = aux.id_categoria;
                    producto.Imagen = aux.imagen;
                    producto.Precio = aux.precio;
                }
            }
            return View(producto);
        }
        //ADmin
        public ActionResult EliminarProduct(int id)
        {
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
                {
                    var producto = db.Productos.Find(id);
                    producto.estado = "0";
                    db.SaveChanges();
                }
            /*List<ProductoTableViewModel> lst = null;
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
            {
                lst = (from d in db.Productos
                       where d.estado.Equals("1")
                       orderby d.nombre_producto
                       select new ProductoTableViewModel
                       {
                           ID = d.id_producto,
                           Nombre = d.nombre_producto,
                           Descripcion = d.descripcion,
                           Precio = d.precio,
                           Categoria = d.Categorias.nombre_categoria,
                           id_categoria = d.id_categoria,
                           Imagen = d.imagen
                       }).ToList();
            }*/
            return RedirectToAction("ProductList", "Product");
        }

        //ADmin editando guardando
        [HttpPost]

        // En este metodo se guardan los datos del producto en la base de datos y se guarda la imagen en la carpeta correspondiente
        public ActionResult AddProduct(int ID, string Nombre, string Descripcion, decimal Precio, int id_categoria, HttpPostedFileBase imagen)
        {
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
            {
                var producto = new Productos();
                    // Asignar los datos de la imagen al modelo
                if (ID != 0) //Actualiza
                {
                        producto = db.Productos.Find(ID);
                        producto.nombre_producto = Nombre;
                        producto.descripcion = Descripcion;
                        producto.precio = Precio;
                        producto.id_categoria = id_categoria;
                    }
                else //Guarda
                {
                        producto.nombre_producto = Nombre;
                        producto.descripcion = Descripcion;
                        producto.precio = Precio;
                        producto.id_categoria = id_categoria;
                        producto.estado = "1";
                    }
                // Verificar si se ha cargado una imagen
                if (imagen != null && imagen.ContentLength > 0)
                {
                    // Leer el archivo de imagen y convertirlo en un arreglo de bytes
                    byte[] imageData = new byte[imagen.ContentLength];
                    imagen.InputStream.Read(imageData, 0, imagen.ContentLength);
                    producto.imagen = imageData;
                }
                if (ID == 0)
                {
                    db.Productos.Add(producto);
                }
                db.SaveChanges();
            }
            return RedirectToAction("ProductList", "Product");
        }

    }

}
