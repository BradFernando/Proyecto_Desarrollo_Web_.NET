using ProyectoBitsBites.Models;
using ProyectoBitsBites.Models.TableViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoBitsBites.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Login(string Msg=null)
        {
            return View(new MsgModel()
            {
                Mensaje = Msg
            }); 
        }

        public ActionResult OutLogin()
        {
            Session["User"] = "";
            Session["email"] = "";
            Session["rol"] = "";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(string User, string Pass)
        {
            try
            {
                using (Models.RestauranteBitsBitesEntities1 db = new Models.RestauranteBitsBitesEntities1())
                {
                    var oUser = (from d in db.Usuarios
                                 where d.correo.Equals(User.Trim()) && d.clave.Equals(Pass.Trim())
                                 select d).FirstOrDefault();

                    if (oUser == null)
                    {
                        ViewBag.Error = "Usuario o contraseña incorrecta";
                        return View(new MsgModel()
                        {
                            Mensaje = "Usuario o contraseña incorrecta"
                        });
                    }

                    Session["User"] = oUser;
                    Session["email"] = oUser.correo;
                    Session["rol"] = oUser.rol;

                    if (oUser.rol.Equals("administrador"))
                    {
                        return RedirectToAction("ProductList", "Product");
                    } 
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult Registro()
        {
            return View(new FormRegistro());
        }

        [HttpPost]
        public ActionResult Registro(string Nombre, string Apellido, string Direccion, string Telefono, string Correo, string Usuario, string Contrasenia)
        {
            using (RestauranteBitsBitesEntities1 db = new RestauranteBitsBitesEntities1())
            {
                var cliente = new Clientes();
                cliente.nombre = Nombre;
                cliente.apellido = Apellido;
                cliente.dirección = Direccion;
                cliente.telefono = Telefono;
                cliente.correo_electronico = Correo;
                db.Clientes.Add(cliente);
                var usuario = new Usuarios();
                usuario.nombre_usuario = Usuario; 
                usuario.clave = Contrasenia;
                usuario.rol = "cliente";
                usuario.correo = Correo;
                db.Usuarios.Add(usuario);
                db.SaveChanges();
            }
            return RedirectToAction("Login", "Acceso");
        }
    }
}