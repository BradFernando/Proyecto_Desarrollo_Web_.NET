using ProyectoBitsBites.Controllers;
using ProyectoBitsBites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoBitsBites.Filters
{
    public class VerificarSession : ActionFilterAttribute
    {
       private Usuarios oUsuario;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);

                oUsuario = (Usuarios)HttpContext.Current.Session["User"];
                if (oUsuario == null)
                {
                    if (filterContext.Controller is AccesoController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("~/Acceso/Login");
                    }
                }
            }  
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }
        }
    }
}